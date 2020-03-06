using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerState {
    public static Dictionary<string, string> getInstance() {
        Dictionary<String, String> state = new Dictionary<String, String>();
        state.Add("hp", "4");
        state.Add("turn", "0");

        return state;
    }
}
