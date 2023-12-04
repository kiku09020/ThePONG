using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extentions {
    public static class RandomExt {
        public static int RandomSign()
        {
            return Random.Range(0, 2) * 2 - 1;
        }
    }
}