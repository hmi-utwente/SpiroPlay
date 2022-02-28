using Spirometry.Debugging;
using Spirometry.SpiroController;
using Spirometry.Statics;
using UnityEngine;

namespace M0_Test
{
    public class M12_Manager : SpiroManager
    {
#pragma warning disable 649

#pragma warning restore 649

        private new void Start()
        {
            base.Start();
            StartCoroutine(generalOverlay.EndInstructions(false));
        }
        private new void Update()
        {
            base.Update();
        }

        protected override void OnStartTest() { }

        protected override void OnEndTest() { }
    
        protected override void OnSwitchToExpiration() { }
        
        protected override void OnReachedProficientFlow() { }
    }
}
