﻿using System;

public class Player
{

    public Card[] Hand  = new Card[2];
    public string Name;
    public int ChipsAmount;
    public int CurrentBet = 0;
    public Boolean Folded = false;
    public HandStrength StrongestHand;
    public User User;



    public Player(string name, int chips ,User user)
        {
        if (chips < 0)
        {
            Logger.Log(Severity.Error, "cant create player With out chips");
            throw new Exception("illegal amount of chips");
        }
        if (name == null)
        {
            Logger.Log(Severity.Exception, "cant create player with null name");
            throw new Exception("illegal player name");
        }
        if(user == null)
        {
            Logger.Log(Severity.Exception, "cant create player with null User");
            throw new Exception("illegal User");
        }
        this.User = user;
        this.Name = name;
        ChipsAmount = chips;
        }

        public void SetCards(Card first, Card second)
        {
        if (first == null || second == null)
        {
            Logger.Log(Severity.Exception, "cant put null cards int player hand");
            throw new Exception("can't put null cards");
        }
            Hand[0] = first;
            Hand[1] = second;
        }

        public void SetBet(int amount)
        {
        if (amount < 0 || amount > ChipsAmount)
        {
            Logger.Log(Severity.Error, "bet must be greater then zero and less-equal to player chips");
            throw new Exception("illegael bet");
        }
            CurrentBet +=amount;
            ChipsAmount -= amount;
        }

        public void ClearBet() { CurrentBet = 0; }

        public void Fold() { Folded = true; }

        public void UndoFold() { Folded = false; }
   }
