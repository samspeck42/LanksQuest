using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public abstract class ActivatingEntity : Entity
    {
        protected List<KeyValuePair<int, string>> activationIds;
        protected List<Activatable> activations;
        protected List<KeyValuePair<int, string>> deactivationIds;
        protected List<Activatable> deactivations;
        protected List<KeyValuePair<int, string>> dependentIds;
        protected List<ActivatingEntity> dependents;
        protected bool addedActivatableEntites = false;
        protected bool hasTriggeredActivations = false;
        public bool HasTriggeredActivations { get { return hasTriggeredActivations; } }

        public ActivatingEntity(GameWorld game, Area area)
            : base(game, area)
        {
            activationIds = new List<KeyValuePair<int, string>>();
            activations = new List<Activatable>();
            deactivationIds = new List<KeyValuePair<int, string>>();
            deactivations = new List<Activatable>();
            dependentIds = new List<KeyValuePair<int, string>>();
            dependents = new List<ActivatingEntity>();
        }

        protected override void processData(Dictionary<string, string> dataDict)
        {
            base.processData(dataDict);

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

        public override void Update(GameTime gameTime)
        {
            if (!addedActivatableEntites)
            {
                foreach (KeyValuePair<int, string> pair in activationIds)
                {
                    Area entityArea;
                    if (pair.Key < 0)
                        entityArea = this.area;
                    else
                        entityArea = this.area.Map.GetAreaByIndex(pair.Key);
                    Entity entity = entityArea.GetEntityById(pair.Value);
                    if (entity != null && entity is Activatable)
                    {
                        activations.Add((Activatable)entity);
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
                    if (entity != null && entity is Activatable)
                    {
                        deactivations.Add((Activatable)entity);
                    }
                }
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
                addedActivatableEntites = true;
            }

            base.Update(gameTime);
        }

        protected void tryToTriggerActivations()
        {
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
                foreach (Activatable activatableEntity in activations)
                {
                    activatableEntity.Activate();
                }
                foreach (Activatable activatableEntity in deactivations)
                {
                    activatableEntity.Deactivate();
                }
            }

            hasTriggeredActivations = true;
        }

        public override void OnEntityCollision(Entity other, HitBox thisHitBox, HitBox otherHitBox)
        {
            
        }

        public override void LoadContent()
        {
        }
    }
}
