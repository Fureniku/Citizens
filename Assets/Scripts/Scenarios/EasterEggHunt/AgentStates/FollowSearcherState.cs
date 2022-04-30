using System;
using Scenarios.EasterEggHunt.Cooperative.Agents;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class FollowSearcherState : EggHunterBaseState {
        
        public FollowSearcherState(EggHunterAgent agent) {
            this.stateName = "Follow Searcher State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            if (agent.GetFollowTarget() != null) {
                float dist = Vector3.Distance(agent.transform.position, agent.GetFollowTarget().transform.position);
                if (dist < agent.GetFollowDistance()) {
                    agent.GetAgent().isStopped = true;
                } else {
                    agent.GetAgent().isStopped = false;
                    agent.ForceAgentDestination(agent.GetFollowTarget());
                }

                if (agent.EggCount() > 0 && !((EggHunterEggRunnerFollow) agent).WaitingForEggs()) {
                    return typeof(TakeEggsToBaseState);
                }
                
            } else {
                return typeof(ReturnToBaseState); //If we're not following an agent we are redundant.
            }

            return null;
        }
        
        public override Type StateEnter() {
            return null;
        }

        public override Type StateExit() {
            return null;
        }
    }
}