﻿using ColossalFramework.UI;
using ICities;
using UnityEngine;
using System;

namespace PopulationDemographics
{
    /// <summary>
    /// handle game loading and unloading
    /// </summary>
    /// <remarks>A new instance of PopulationDemographicsLoading is NOT created when loading a game from the Pause Menu.</remarks>
    public class PopulationDemographicsLoading : LoadingExtensionBase
    {
        // a panel to display the population demographics
        public static PopulationDemographicsPanel panel;

        // a button to display the panel
        private UIButton _demographics;

        // a label that follows the cursor
        public static UILabel cursorLabel;

        public override void OnLevelLoaded(LoadMode mode)
        {
            // do base processing
            base.OnLevelLoaded(mode);

            try
            {
                // check for new or loaded game
                if (mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario || mode == LoadMode.LoadGame)
                {
                    // get the PopulationInfoViewPanel panel (displayed when the user clicks on the Population info view button)
                    PopulationInfoViewPanel populationPanel = UIView.library.Get<PopulationInfoViewPanel>(typeof(PopulationInfoViewPanel).Name);
                    if (populationPanel == null)
                    {
                        LogUtil.LogError("Unable to find PopulationInfoViewPanel.");
                        return;
                    }

                    // create a new PopulationDemographicsPanel which will eventually trigger the panel's Start method
                    panel = populationPanel.component.AddUIComponent<PopulationDemographicsPanel>();
                    if (panel == null)
                    {
                        LogUtil.LogError("Unable to create Population Demographics panel on PopulationInfoViewPanel.");
                        return;
                    }

                    // create button to show the panel
                    _demographics = populationPanel.component.AddUIComponent<UIButton>();
                    if (_demographics == null)
                    {
                        LogUtil.LogError("Unable to create Demographics button on PopulationInfoViewPanel.");
                        return;
                    }
                    _demographics.name = "Demographics";
                    _demographics.text = "Demographics";
                    _demographics.textScale = 0.75f;
                    _demographics.horizontalAlignment = UIHorizontalAlignment.Center;
                    _demographics.textVerticalAlignment = UIVerticalAlignment.Middle;
                    _demographics.autoSize = false;
                    _demographics.size = new Vector2(120f, 20f);
                    _demographics.relativePosition = new Vector3(220f, 290f);
                    _demographics.normalBgSprite = "ButtonMenu";
                    _demographics.hoveredBgSprite = "ButtonMenuHovered";
                    _demographics.pressedBgSprite = "ButtonMenuPressed";
                    _demographics.isVisible = true;
                    _demographics.eventClicked += Demographics_eventClicked;

                    // create cursor label
                    cursorLabel = (UILabel)UIView.GetAView().AddUIComponent(typeof(UILabel));
                    if (cursorLabel == null)
                    {
                        LogUtil.LogError("Unable to create cursor label on main view.");
                        return;
                    }
                    cursorLabel.name = "PopulationDemographicsCursorLabel";
                    cursorLabel.text = "000.0";
                    cursorLabel.textColor = Color.cyan;
                    cursorLabel.outlineColor = Color.black;
                    cursorLabel.outlineSize = 1;
                    cursorLabel.useOutline = true;
                    cursorLabel.textAlignment = UIHorizontalAlignment.Left;
                    cursorLabel.textScale = 0.75f;
                    cursorLabel.opacity = 1f;       // by not setting a background, the background is transparent and opacity is for only the text
                    cursorLabel.canFocus = false;
                    cursorLabel.autoSize = false;
                    cursorLabel.size = new Vector2(50f, 15f);
                    cursorLabel.relativePosition = new Vector3(0f, 0f);
                    cursorLabel.isVisible = false;  // start hidden

                    // create the Harmony patches
                    if (!BuildingAIPatch.CreatePatches()) { return; }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }

        /// <summary>
        /// handle Demographics button clicked
        /// </summary>
        private void Demographics_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            // toggle the panel visibility
            panel.isVisible = !panel.isVisible;
            Configuration.SavePanelVisible(panel.isVisible);
        }

        public override void OnLevelUnloading()
        {
            // do base processing
            base.OnLevelUnloading();

            try
            {
                // remove Harmony patches
                BuildingAIPatch.RemovePatches();

                // destroy the objects added directly to the PopulationInfoViewPanel
                // must do this explicitly because loading a saved game from the Pause Menu
                // does not destroy the objects implicitly like returning to the Main Menu to load a saved game
                if (cursorLabel != null)
                {
                    UnityEngine.Object.Destroy(cursorLabel);
                    cursorLabel = null;
                }
                if (_demographics != null)
                {
                    UnityEngine.Object.Destroy(_demographics);
                    _demographics = null;
                }
                if (panel != null)
                {
                    UnityEngine.Object.Destroy(panel);
                    panel = null;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogException(ex);
            }
        }
    }
}