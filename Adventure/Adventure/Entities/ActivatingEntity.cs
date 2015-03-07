using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class ActivatingEntity : Entity
    {
        public bool HasTriggeredActivations { get { return hasTriggeredActivations; } }

        protected List<KeyValuePair<int, string>> activationIds = new List<KeyValuePair<int, string>>();
        protected List<KeyValuePair<int, string>> deactivationIds = new List<KeyValuePair<int, string>>();
        protected List<KeyValuePair<int, string>> dependentIds = new List<KeyValuePair<int, string>>();
        protected bool hasTriggeredActivations = false;

        public ActivatingEntity(GameWorld game, Area area)
            : base(game, area)
        { }

        protected override void processAttributeData(Dictionary<string, string> dataDict)
        {
            base.processAttributeData(dataDict);

            if (dataDict.ContainsKey("activations"))
            {
                string[] activationIdsData = dataDict["activations"].Split(';');
                foreach (string str in activationIdsData)
                {
                    activationIds.Add(parseIdString(str));
                }
            }
            if (dataDict.ContainsKey("deactivations"))
            {
                string[] deactivationIdsData = dataDict["deactivations"].Split(';');
                foreach (string str in deactivationIdsData)
                {
                    deactivationIds.Add(parseIdString(str));
                }
            }
            if (dataDict.ContainsKey("dependents"))
            {
                string[] dependentCellsData = dataDict["dependents"].Split(';');
                foreach (string str in dependentCellsData)
                {
                    dependentIds.Add(parseIdString(str));
                }
            }
        }

        private static KeyValuePair<int,string> parseIdString(string str)
        {
            string[] idData = str.Split(',');
            string id = idData[0];
            int areaNum = idData.Length > 1 ? int.Parse(idData[1].Trim()) : -1;
            return new KeyValuePair<int, string>(areaNum, id);
        }

        protected void tryToTriggerActivations()
        {
            List<ActivatingEntity> dependents = new List<ActivatingEntity>();
            foreach (KeyValuePair<int, string> pair in dependentIds)
            {
                Area entityArea;
                if (pair.Key < 0)
                    entityArea = this.area;
                else
                    entityArea = this.area.Map.GetAreaByIndex(pair.Key);
                Entity entity = entityArea.GetEntityById(pair.Value);
                if (entity != null && entity is ActivatingEntity)
                {
                    dependents.Add((ActivatingEntity)entity);
                }
            }

            bool canTrigger = true;
            foreach (ActivatingEntity dependent in dependents)
            {
                if (!dependent.HasTriggeredActivations)
                {
                    canTrigger = false;
                    break;
                }
            }

            if (canTrigger)
            {
                List<Triggerable> activations = new List<Triggerable>();
                List<Triggerable> deactivations = new List<Triggerable>();

                foreach (KeyValuePair<int, string> pair in activationIds)
                {
                    Area entityArea;
                    if (pair.Key < 0)
                        entityArea = this.area;
                    else
                        entityArea = this.area.Map.GetAreaByIndex(pair.Key);
                    Entity entity = entityArea.GetEntityById(pair.Value);
                    if (entity != null && entity is Triggerable)
                    {
                        activations.Add((Triggerable)entity);
                    }
                }
                foreach (KeyValuePair<int, string> pair in deactivationIds)
                {
                    Area entityArea;
                    if (pair.Key < 0)
                        entityArea = this.area;
                    else
                        entityArea = this.area.Map.GetAreaByIndex(pair.Key);
                    Entity entity = entityArea.GetEntityById(pair.Value);
                    if (entity != null && entity is Triggerable)
                    {
                        deactivations.Add((Triggerable)entity);
                    }
                }

                foreach (Triggerable activatableEntity in activations)
                {
                    activatableEntity.TriggerOn();
                }
                foreach (Triggerable activatableEntity in deactivations)
                {
                    activatableEntity.TriggerOff();
                }
            }

            hasTriggeredActivations = true;
        }
    }
}
