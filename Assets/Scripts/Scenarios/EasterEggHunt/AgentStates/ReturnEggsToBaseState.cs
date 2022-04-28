﻿using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ReturnEggsToBaseState : EggHunterBaseState {
        
        public ReturnEggsToBaseState(EggHunterAgent agent) {
            this.stateName = "Return Eggs To Base State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (Vector3.Distance(agent.transform.position, agent.GetScenarioManager().GetDepositPoint().transform.position) < 5.0f) {
                if (agent.EggCount() > 0) {
                    ((EggHunterScenarioManager) agent.GetScenarioManager()).DepositEggs(agent.EggCount());
                    agent.RemoveEggs(agent.EggCount());
                }

                return typeof(MoveToLocationState);
            }

            return null;
        }
        
        public override Type StateEnter() {
            agent.SetAgentDestination(agent.GetScenarioManager().GetDepositPoint());
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}