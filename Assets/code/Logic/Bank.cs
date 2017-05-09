﻿using UnityEngine;
using System.Collections;
using System;

public class Bank
{
    Wallet reservs = new Wallet(0);
    Value givenLoans = new Value(0);
    internal void PutOnDeposit(Wallet fromWho, Value howMuch)
    {
        fromWho.pay(reservs, howMuch);
    }
    internal void TakeFromDeposit(Wallet forWho, Value howMuch)
    {
        reservs.pay(forWho, howMuch);
    }
    /// <summary>
    /// checks are outside
    /// </summary>   
    internal void TakeLoan(Producer taker, Value howMuch)
    {
        reservs.pay(taker.wallet, howMuch);
        taker.loans.add(howMuch);
        this.givenLoans.add(howMuch);
    }
    /// <summary>
    /// checks are outside
    /// </summary>
    internal void returnLoan(Producer returner, Value howMuch)
    {
        //reservs.pay(taker.wallet, howMuch);
        returner.wallet.pay(reservs, howMuch);
        returner.loans.subtract(howMuch);
        this.givenLoans.subtract(howMuch);
    }
    internal Value getGivenLoans()
    {
        return new Value(givenLoans.get());
    }
    internal float getReservs()
    {
        return reservs.haveMoney.get();
    }

    internal bool CanITakeThisLoan(Value loan)
    {
        //if there is enough money and enough reserves
        if (reservs.haveMoney.get() - loan.get() >= getMinimalReservs().get())
            return true;
        return false;
    }

    private Value getMinimalReservs()
    {
        //todo improve reserves
        return new Value(100f);
    }

    override public string ToString()
    {
        return reservs.ToString();
    }

    internal void defaultLoaner(Producer producer)
    {
        givenLoans.subtract(producer.loans);
        producer.loans.set(0);
    }
    /// <summary>
    /// Assuming all clients already defaulted theirs loans
    /// </summary>    
    internal void add(Bank annexingBank)
    {
        annexingBank.reservs.sendAll(this.reservs);
        annexingBank.givenLoans.sendAll(this.givenLoans);        
    }
}
