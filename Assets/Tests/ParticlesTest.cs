using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ParticlesTest : MonoBehaviour
    {
        
        private void Update()
        {
            
        }


        [Test]
        public void SimplePasses()
        {
            Debug.Log("Play mode test simple");
        }

        private static int frameCounter = 0;
        [UnityTest]
        public IEnumerator EnumeratorPasses()
        {
            while (true)
            {
                Debug.Log("Play mode test enumerator, frame "+frameCounter);
                frameCounter++;
                yield return null;    
            }
        }
    }
}
