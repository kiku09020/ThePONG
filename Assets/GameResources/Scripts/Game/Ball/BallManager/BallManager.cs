using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ball.Management
{
    public class BallManager : MonoBehaviour
    {
        /* Fields */
        [Header("Generating")]
        [SerializeField] BallPool ballPool;
        [SerializeField] Vector2 generatePos;

        [Header("Shoot")]
        [SerializeField] float minDirAngle = .25f;
        [SerializeField] float maxDirAngle = .75f;

        int ballShootCount;

        //-------------------------------------------------------------------
        /* Properties */

        //-------------------------------------------------------------------
        /* Messages */

        //-------------------------------------------------------------------
        /* Methods */
        public BallCore GenerateBall()
        {
            var ball = ballPool.Pool.Get();
            var core = ball.Core;

            core.transform.position = generatePos;

            return core;
        }

        public void ShotBall(BallCore core)
        {
            core.Shot(GetRandomDir());

            ballShootCount++;
        }

        // ランダムな方向を取得
        Vector2 GetRandomDir()
        {
            var randomHorSign = (ballShootCount % 2 == 0) ? 1 : -1;         // 横方向は交互に変更
            var randomVerSign = Extentions.RandomExt.RandomSign();          // 縦方向ランダム
            var randomVerDir = Random.Range(minDirAngle, maxDirAngle);

            return new Vector2(randomHorSign, randomVerSign * randomVerDir);
        }

    }
}