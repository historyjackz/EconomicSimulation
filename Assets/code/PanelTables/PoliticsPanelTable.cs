﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

// represen each opunit record in table

public class PoliticsPanelTable : MyTable
{
    override protected void Refresh()
    {
        ////if (Game.date != 0)
        {
            base.RemoveButtons();
            AddButtons();
            contentPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentPanel.childCount / this.columnsAmount * rowHeight + 50);
        }
    }
    protected void AddButton(string text, AbstractReform type)
    {
        GameObject newButton = buttonObjectPool.GetObject();
        //newButton.transform.SetParent(contentPanel, false);
        newButton.transform.SetParent(contentPanel);
        SampleButton sampleButton = newButton.GetComponent<SampleButton>();
        //if (inventionType == null)
        //    sampleButton.Setup(text, this, null);
        //else
        sampleButton.Setup(text, this, type);
    }
    override protected void AddButtons()
    {
        int counter = 0;

        // Adding reform name
        AddButton("Reform", null);

        ////Adding Status
        AddButton("Status", null);

        ////Adding Can change possibility
        AddButton("Can change", null);

        if (Game.player != null)
        {
            //var factoryList = Game.player;

            foreach (var next in Game.player.reforms)
               // if (next.isAvailable(Game.player))
                {
                    // Adding shownFactory type
                    AddButton(next.ToString(), next);

                    ////Adding potential output
                    AddButton(next.getValue().ToString(), next);
                    ////Adding availability
                    if (next.canChange())
                        AddButton("Yep", next);
                    else
                        AddButton("Nope", next);

                    counter++;
                }
        }
    }
}