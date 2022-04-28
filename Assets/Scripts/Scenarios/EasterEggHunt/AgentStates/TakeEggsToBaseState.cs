﻿using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class TakeEggsToBaseState : EggHunterBaseState {
        
        public TakeEggsToBaseState(EggHunterAgent agent) {
            this.stateName = "Take Eggs To Base State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            float dist = Vector3.Distance(agent.transform.position, agent.GetScenarioManager().GetDepositPoint().transform.position);

            if (dist < 2.0f) {
                if (agent.EggCount() > 0) {
                    ((EggHunterCooperativeScenarioManager) agent.GetScenarioManager()).DepositEggs(agent.EggCount());
                    agent.RemoveEggs(agent.EggCount());
                }

                return typeof(FollowSearcherState);
            }

            return null;
        }
        
        public override Type StateEnter() {
            return null;
        }

        public override Type StateExit() {
            agent.SetAgentDestination(agent.GetFollowTarget());
            ((EggHunterEggRunnerFollow) agent).SetWaitingForEggs();
            return null;
        }
    }
}