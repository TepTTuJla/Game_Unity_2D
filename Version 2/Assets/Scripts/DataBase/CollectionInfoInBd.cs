﻿using System;
using UnityEngine;
namespace DataBase
{
    public class CollectionInfoInBd : MonoBehaviour
    {
        public float incomingDamage;
        public float outcomingDamage;
        public int countOpenChest;
        public int countKillBlackBandit;
        public int countKillWhiteBandit;
        public int countKillBoss;
        public int rating;
        public float time;
        public int death;

        //Вызывается во время 1 кадра
        private void Start()
        {
            time = 0;
        }
        
        //Вызывается постоянно во время каждого кадра
        private void Update()
        {
            KillBoss();
            time += Time.deltaTime;
        }
        
        public void TakingDamage(float damage)
        {
            incomingDamage += damage;
        }

        public void Attack(float damage)
        {
            outcomingDamage += damage;
        }

        public void OpenChest()
        {
            countOpenChest++;
        }

        public void KillBlackBandit()
        {
            countKillBlackBandit++;
        }

        public void KillWhiteBandit()
        {
            countKillWhiteBandit++;
        }

        public void KillBoss()
        {
            countKillBoss++;
        }

        public void SetDeath()
        {
            death++;
        }

        private void CountRating()
        {
            var ratingCount = incomingDamage + countKillBlackBandit * 15f + countKillWhiteBandit * 50f +
                              countKillBoss * 200f + countOpenChest * 5f;
            if (incomingDamage == 0 && countKillBoss > 0) ratingCount *= 1.3f;
            switch (death)
            {
                case 1:
                    ratingCount /= 1.1f;
                    break;
                case 2:
                    ratingCount /= 1.3f;
                    break;
            }

            rating = Convert.ToInt32(ratingCount);
            if (rating < 0) rating = 0;
        }

        public void SetInBd()
        {
            CountRating();
            var incDamage = Convert.ToInt32(incomingDamage);
            var outDamage = Convert.ToInt32(outcomingDamage);
            MyDataBase.AddCompletionInBd(SetCompletionInTable.id, incDamage, outDamage,
                countKillWhiteBandit, countKillBlackBandit, countKillBoss, rating, countOpenChest, time);

            incomingDamage = 0;
            outcomingDamage = 0;
            countKillBlackBandit = 0;
            countKillWhiteBandit = 0;
            countKillBoss = 0;
            countOpenChest = 0;
            rating = 0;
            time = 0;
            death = 0;
        }
    }
}