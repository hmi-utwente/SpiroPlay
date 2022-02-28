using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    [Header("Fire Effect")]
    [SerializeField] private ParticleSystem flames = null;
    [SerializeField] private ParticleSystem flamesCombust = null;

    private ParticleSystem ps;
    private ParticleSystem psCombust;
    public Animator fireAnimator;
    [HideInInspector] public bool isLit = false;
    private bool _hasCombusted = false;

    private void Start()
    {
        if (fireAnimator == null)
        {
            ps = Instantiate(flames, transform.position, transform.rotation);
            psCombust = Instantiate(flamesCombust, transform.position, transform.rotation);
        }
        

        if (ps == null || psCombust == null) return;
        ps.Stop();
        ps.Clear();
        psCombust.Stop();
        psCombust.Clear();
    }
    public void Light()
    {
        if (isLit) return;
        isLit = true;
        print("hasCombusted: " + _hasCombusted);
        if (fireAnimator != null)
        {
            if (!_hasCombusted)
            {
                fireAnimator.SetTrigger("normal_fire");
            }
            else
            {
                fireAnimator.SetTrigger("normal_fire");
                fireAnimator.SetTrigger("blue_fire");
            }
        }

        if (psCombust == null || ps == null) return;
        if (!isLit)
        {
            if (!_hasCombusted)
            {
                ps.Play();
            }
            else
            {
                psCombust.Play();
            }
        }
    }

    public void Combust()
    {
        if (_hasCombusted) return;
        _hasCombusted = true;
        if (!isLit) return;
        if (fireAnimator != null)
        {
            fireAnimator.SetTrigger("blue_fire");
        }
        else
        {
            ps.Stop();
            psCombust.Play();
        }
    }
}
