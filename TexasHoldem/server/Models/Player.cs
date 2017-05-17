﻿using System.Collections.Generic;

namespace server.Models
{
    public class Player
    {
        public string PlayerName;
        public int CurrentBet;
        public int ChipsAmount;
        public string Avatar;
        public string[] PlayerHand;
        public List<string> messages = new List<string>();
    }
}