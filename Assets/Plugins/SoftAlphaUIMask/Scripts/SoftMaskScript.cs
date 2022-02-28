using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace RandomchaosLtd.SAUIM
{

    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/SoftAlphaUIMask/UI/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        Material mat;
        Canvas canvas;

        [Tooltip("The area that is to be used as the container.")]
        public RectTransform MaskArea;
        RectTransform myRect;

        [Tooltip("Texture to be used to do the soft alpha")]
        public Texture2D AlphaMask;

        [Tooltip("At what point to apply the alpha min range 0-1")]
        [Range(0, 1)]
        public float CutOff = 0;

        [Tooltip("Implement a hard blend based on the Cutoff")]
        public bool HardBlend = false;

        [Tooltip("Flip the masks alpha value")]
        public bool FlipAlphaMask = false;

        [Tooltip("A Rect Transform that can be used to scale and move the mask - Does not apply to Text UI Components being masked")]
        public RectTransform maskScalingRect;

        [Tooltip("If Mask Scals Rect is given, and this value is true, the area around the mask will not be clipped")]
        public bool DontClipMaskScalingRect = false;

        [Tooltip("If set to true, this mask is applied to all child Text and Graphic objects belonging to this object.")]
        public bool CascadeToALLChildren;

        Vector3[] worldCorners;

        Vector2 AlphaUV;

        Vector2 min;
        Vector2 max = Vector2.one;
        Vector2 p;
        Vector2 siz;

        bool MaterialNotSupported; // UI items like toggles, we can stil lcascade down to them though :)


        Rect maskRect;
        Rect contentRect;

        Vector2 centre;

        bool isText = false;

        // Use this for initialization
        void Start()
        {
            myRect = GetComponent<RectTransform>();

            if (!MaskArea)
                MaskArea = myRect;

            if (GetComponent<Graphic>() != null)
            {
                mat = new Material(Shader.Find("SoftAlphaUIMask/UI/SoftMaskShader"));
                GetComponent<Graphic>().material = mat;
            }

            if (GetComponent<Text>())
            {
                isText = true;
                mat = new Material(Shader.Find("SoftAlphaUIMask/UI/SoftMaskShaderText"));
                GetComponent<Text>().material = mat;

                GetCanvas();

                // For some reason, having the mask control on the parent and disabled stops the mouse interacting
                // with the texture layer that is not visible.. Not needed for the Image.
                if (transform.parent.GetComponent<Button>() == null && transform.parent.GetComponent<Mask>() == null)
                    transform.parent.gameObject.AddComponent<Mask>();

                if (transform.parent.GetComponent<Mask>() != null)
                    transform.parent.GetComponent<Mask>().enabled = false;
            }

            if (CascadeToALLChildren)
            {
                for (int c = 0; c < transform.childCount; c++)
                {
                    SetSAM(transform.GetChild(c));
                }
            }

            MaterialNotSupported = mat == null;
        }

        void SetSAM(Transform t)
        {
            SoftMaskScript thisSam = t.gameObject.GetComponent<SoftMaskScript>();
            if (thisSam == null)
            {
                thisSam = t.gameObject.AddComponent<SoftMaskScript>();
            }
            thisSam.MaskArea = MaskArea;
            thisSam.AlphaMask = AlphaMask;
            thisSam.CutOff = CutOff;
            thisSam.HardBlend = HardBlend;
            thisSam.FlipAlphaMask = FlipAlphaMask;
            thisSam.maskScalingRect = maskScalingRect;
            thisSam.DontClipMaskScalingRect = DontClipMaskScalingRect;
            thisSam.CascadeToALLChildren = CascadeToALLChildren;
        }


        void GetCanvas()
        {
            Transform t = transform;

            int lvlLimit = 100;
            int lvl = 0;

            while (canvas == null && lvl < lvlLimit)
            {
                canvas = t.gameObject.GetComponent<Canvas>();
                if (canvas == null)
                    t = t.parent;

                lvl++;
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetMask();
        }

        void SetMask()
        {
            if (MaterialNotSupported)
                return;

            // Get the two rectangle areas
            maskRect = MaskArea.rect;

            contentRect = myRect.rect;

            if (isText) // Need to do our calculations in world for Text
            {
                maskScalingRect = null;
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && Application.isPlaying)
                {
                    p = canvas.transform.InverseTransformPoint(MaskArea.transform.position);
                    siz = new Vector2(maskRect.width, maskRect.height);
                }
                else
                {
                    worldCorners = new Vector3[4];
                    MaskArea.GetWorldCorners(worldCorners);
                    siz = (worldCorners[2] - worldCorners[0]);
                    p = MaskArea.transform.position;
                }

                min = p - (new Vector2(siz.x, siz.y) * .5f);
                max = p + (new Vector2(siz.x, siz.y) * .5f);

            }
            else // Need to do our calculations in tex space for Image.
            {
                if (maskScalingRect != null)
                    maskRect = maskScalingRect.rect;

                // Get the centre offset - Fix suggested by Doron Assayas Terre
                centre = myRect.transform.InverseTransformPoint(MaskArea.transform.TransformPoint(MaskArea.rect.center));

                if (maskScalingRect != null)
                    centre = myRect.transform.InverseTransformPoint(maskScalingRect.transform.TransformPoint(maskScalingRect.rect.center));

                centre += (Vector2)myRect.transform.InverseTransformPoint(myRect.transform.position) - myRect.rect.center;

                // Set the scale for mapping texcoords mask
                AlphaUV = new Vector2(maskRect.width / contentRect.width, maskRect.height / contentRect.height);

                // set my min and max to the centre offest
                min = centre;
                max = min;

                siz = new Vector2(maskRect.width, maskRect.height) * .5f;
                // Move them out to the min max extreams
                min -= siz;
                max += siz;

                // Now move these into texture space. 0 - 1
                min = new Vector2(min.x / contentRect.width, min.y / contentRect.height) + new Vector2(.5f, .5f);
                max = new Vector2(max.x / contentRect.width, max.y / contentRect.height) + new Vector2(.5f, .5f);


            }

            mat.SetFloat("_HardBlend", HardBlend ? 1 : 0);

            // Pass the values to the shader
            mat.SetVector("_Min", min);
            mat.SetVector("_Max", max);

            mat.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
            mat.SetTexture("_AlphaMask", AlphaMask);

            mat.SetInt("_NoOuterClip", DontClipMaskScalingRect && maskScalingRect != null ? 1 : 0);

            if (!isText) // No mod needed for Text
                mat.SetVector("_AlphaUV", AlphaUV);

            mat.SetFloat("_CutOff", CutOff);
        }

    }
}