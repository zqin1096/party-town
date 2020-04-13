using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMap {
    public static Card GetCardInstance(string cardNo) {
        // Debug.LogFormat("CardMap.GetCardInstance(): cardNo: {0}", cardNo);

        switch (cardNo) {
            case "0":
                return new AttackCard();
            case "1":
                return new HealCard();
            case "2":
                return new DefenseCard();
            case "3":
                return new TakeCard();
            case "4":
                return new SpecialAttack();
            case "5":
                return new BillizardCard();
            default:
                throw new System.InvalidOperationException("CardMap.GetCardInstance(): cardNo wrong");
        }
    }
}
