﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Nashet.ValueSpace;

namespace Nashet.EconomicSimulation
{
    public class World : MonoBehaviour
    {
        static public IEnumerable<IInvestable> GetAllAllowedInvestments(Country includingCountry, Agent investor)
        {
            var countriesAllowingInvestments = Country.getAllExisting().Where(x => x.economy.getTypedValue().AllowForeignInvestments || x == includingCountry);
            foreach (var country in countriesAllowingInvestments)
                foreach (var item in country.GetAllInvestmentProjects(investor))
                    yield return item;
        }

        internal static IEnumerable<Factory> GetAllFactories()
        {
            foreach (var item in Country.getAllExisting())
                foreach (var factory in item.getAllFactories())
                    yield return factory;
        }
        // temporally
        internal static IEnumerable<KeyValuePair<IShareOwner, Procent>> GetAllShares()
        {
            foreach (var item in Country.getAllExisting())
                foreach (var factory in item.getAllFactories())
                    foreach (var record in factory.ownership.GetAllShares())
                        yield return record;
        }
        // temporally
        internal static IEnumerable<KeyValuePair<IShareable, Procent>> GetAllShares(IShareOwner owner)
        {
            foreach (var item in Country.getAllExisting())
                foreach (var factory in item.getAllFactories())
                    foreach (var record in factory.ownership.GetAllShares())
                        if (record.Key == owner)
                            yield return new KeyValuePair<IShareable, Procent>(factory, record.Value);
        }
    }
}