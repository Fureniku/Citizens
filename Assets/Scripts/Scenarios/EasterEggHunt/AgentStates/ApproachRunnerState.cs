using System;
using UnityEngine;

namespace Scenarios.EasterEggHunt.AgentStates {
    public class ApproachRunnerState : EggHunterBaseState {
        
        public ApproachRunnerState(EggHunterCooperativePairSearch agent) {
            this.stateName = "Approach Runner State";
            this.agent = agent;
        }
        
        public override Type StateUpdate() {
            EggHunterEggRunnerFollow runner = ((EggHunterCooperativePairSearch) agent).GetRunner().GetComponent<EggHunterEggRunnerFollow>();
            float dist = Vector3.Distance(agent.transform.position, runner.transform.position);

            if (dist < 2.0f) {
                agent.GiveEgg(runner);
                if (agent.EggCount() == 0) {
                    ((EggHunterCooperativePairSearch) agent).TellNoMoreEggs(runner);
                    agent.FinishedSearch();
                    if (agent.ReturnToBase()) {
                        return typeof(ReturnToBaseState);
                    }

                    return typeof(MoveToLocationState);
                }
            } else {
                agent.SetAgentDestination(runner.gameObject);
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