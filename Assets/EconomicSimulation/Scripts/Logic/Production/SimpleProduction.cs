﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nashet.ValueSpace;
namespace Nashet.EconomicSimulation
{
    /// <summary>
    /// Contains common mechanics for Factory and ArtisanProduction
    /// </summary>
    abstract public class SimpleProduction : Producer
    {
        //private Agent owner;
        private readonly FactoryType type;
        private readonly StorageSet inputProductsReserve = new StorageSet();

        protected SimpleProduction(FactoryType type, Province province) : base(province)
        {
            this.type = type;
            //gainGoodsThisTurn = new Storage(this.getType().basicProduction.getProduct());
            //storage = new Storage(this.getType().basicProduction.getProduct());
            //sentToMarket = new Storage(this.getType().basicProduction.getProduct());
            changeProductionType(this.getType().basicProduction.getProduct());
        }
        //internal Agent getOwner()
        //{
        //    return owner;
        //}
        //public void setOwner(Agent agent)
        //{
        //    owner = agent;
        //}
        public StorageSet getInputProductsReserve()
        {
            return inputProductsReserve;
        }
        public FactoryType getType()
        {
            return type;
        }
        override public string ToString()
        {
            return "crafting " + getType().basicProduction;
        }
       
        public override void simulate()
        {
            throw new NotImplementedException();
        }
        override public void SetStatisticToZero()
        {
            base.SetStatisticToZero();
            storage.set(0f);
        }
        virtual internal float getProfit()
        {
            return moneyIncomethisTurn.get() - getExpences();
        }

        /// <summary>
        /// Fills storageNow and gainGoodsThisTurn. Don't not to confuse with Producer.produce()
        /// </summary>
        protected void produce(Value multiplier)
        {
            addProduct(getType().basicProduction.Copy().Multiply(multiplier));
            if (getGainGoodsThisTurn().isNotZero())
            {
                storage.add(getGainGoodsThisTurn());
                calcStatistics();
            }
            //consume Input Resources
            if (!getType().isResourceGathering())
                foreach (Storage next in getRealAllNeeds())
                    if (next.isAbstractProduct())
                    {
                        var substitute = getInputProductsReserve().convertToBiggestStorage(next);
                        if (substitute.isNotZero())
                            getInputProductsReserve().Subtract(substitute, false); // could be zero reserves if isJustHiredPeople() 
                    }
                    else
                        getInputProductsReserve().Subtract(next, false);
        }
        abstract internal Procent getInputFactor();
        protected Procent getInputFactor(Procent multiplier)
        {
            if (multiplier.isZero())
                return Procent.ZeroProcent.Copy();
            if (getType().isResourceGathering())
                return Procent.HundredProcent.Copy();

            List<Storage> reallyNeedResources = new List<Storage>();
            //Storage available;

            // how much we really want
            foreach (Storage input in getType().resourceInput)
            {
                reallyNeedResources.Add(input.Copy().Multiply(multiplier));
            }

            // checking if there is enough in market
            //old DSB
            //foreach (Storage input in realInput)
            //{
            //    available = Game.market.HowMuchAvailable(input);
            //    if (available.get() < input.get())
            //        input.set(available);
            //}

            // check if we have enough resources
            foreach (Storage resource in reallyNeedResources)
            {
                Storage haveResource = getInputProductsReserve().getBiggestStorage(resource.getProduct());
                //if (!getInputProductsReserve().has(resource))
                if (haveResource.isSmallerThan(resource))
                {
                    // what we really have
                    resource.set(haveResource);
                }
            }
            //old last turn consumption checking thing
            //foreach (Storage input in realInput)
            //{

            //    //if (Game.market.getDemandSupplyBalance(input.getProduct()) >= 1f)
            //    //available = input

            //    available = consumedLastTurn.findStorage(input.getProduct());
            //    if (available == null)
            //        ;// do nothing - pretend there is 100%, it fires only on shownFactory start
            //    else
            //    if (!justHiredPeople && available.get() < input.get())
            //        input.set(available);
            //}
            // checking if there is enough money to pay for
            // doesn't have sense with inputReserv
            //foreach (Storage input in realInput)
            //{
            //    Storage howMuchCan = wallet.HowMuchCanAfford(input);
            //    input.set(howMuchCan.get());
            //}
            var inputFactor = Procent.HundredProcent.Copy();
            // searching lowest factor
            foreach (Storage need in reallyNeedResources)
            {
                Value denominator = getType().resourceInput.GetFirstSubstituteStorage(need.getProduct()).Copy().multiply( multiplier);
                if (denominator.isNotZero()) 
                {
                    var newfactor = new Procent(need, denominator);
                    if (newfactor.isSmallerThan(inputFactor))
                        inputFactor = newfactor;
                }
                else // no resources
                    inputFactor.set(0f);
            }
            return inputFactor;
        }
        abstract public List<Storage> getHowMuchInputProductsReservesWants();
        protected List<Storage> getHowMuchInputProductsReservesWants(Value multiplier)
        {
            //Value multiplier = new Value(getWorkForceFulFilling() * getLevel() * Options.FactoryInputReservInDays);
            if (getType().isResourceGathering())
                return null;
            List<Storage> result = new List<Storage>();

            foreach (Storage next in getType().resourceInput)
            {
                Storage howMuchWantToBuy = new Storage(next);
                howMuchWantToBuy.multiply(multiplier);
                Storage howMuchHave = getInputProductsReserve().getBiggestStorage(next.getProduct());
                if (howMuchWantToBuy.isBiggerThan(howMuchHave))
                {
                    howMuchWantToBuy.subtract(howMuchHave);
                    result.Add(howMuchWantToBuy);
                }//else  - there is enough reserves, you shouldn't buy than   
            }
            return result;
        }
        // Should remove market availability assumption since its goes to double- calculation?
        //public List<Storage> getRealNeeds()
        //{
        //    Value multiplier = new Value(getEfficiency(false).get() * getLevel());

        //    List<Storage> result = new List<Storage>();

        //    foreach (Storage next in getType().resourceInput)
        //    {
        //        Storage nStor = new Storage(next.getProduct(), next.get());
        //        nStor.multiple(multiplier);
        //        result.Add(nStor);
        //    }
        //    return result;
        //}

        protected List<Storage> getRealNeeds(Value multiplier)
        {
            //Value multiplier = new Value(getEfficiency(false).get() * getLevel());
            if (getType().isResourceGathering())
                return null;
            List<Storage> result = new List<Storage>();

            foreach (Storage next in getType().resourceInput)
            {
                Storage nStor = new Storage(next.getProduct(), next.get());
                nStor.multiply(multiplier);
                result.Add(nStor);
            }
            return result;
        }

        /// <summary>  Return in pieces basing on current prices and needs  /// </summary>        
        protected float getLocalEffectiveDemand(Product product, Procent multiplier)
        {
            // need to know how much i Consumed inside my needs
            Storage need = getType().resourceInput.GetFirstSubstituteStorage(product).Copy();
            if (need.isZero())
                return 0f;
            else
            {                
                Storage realNeed = need.Multiply(multiplier.get());             
                Storage canAfford = howMuchCanAfford(realNeed);
                return canAfford.get();
            }
        }
        virtual internal float getExpences()
        {
            return Game.market.getCost(getConsumed()).get();
        }
        public bool isAllInputProductsCollected()
        {
            var realNeeds = getRealAllNeeds();
            foreach (var item in realNeeds)
            {
                if (!inputProductsReserve.has(item))
                    return false;
            }
            return true;
        }
    }
}