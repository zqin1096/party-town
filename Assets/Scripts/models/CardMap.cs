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
      default:
          throw new System.InvalidOperationException("CardMap.GetCardInstance(): cardNo wrong");
    }
  }

  // public static Action<PlayerController, PlayerController> GetCardEffect(string cardNo) {
  //   // Debug.LogFormat("CardMap.GetCardEffect(): cardNo: {0}", cardNo);

  //   switch (cardNo) {
  //     case "0":
  //         return new AttackCard().Effect();
  //     case "1":
  //         return new HealCard().Effect();
  //     default:
  //         throw new System.InvalidOperationException("CardMap.GetCardEffect(): cardNo wrong");
  //   }
  // }
}