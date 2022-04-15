using System;

public class WalkState : PedestrianBaseState {

    public WalkState(PedestrianAgent agent) {
        this.stateName = "Walking State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetCurrentTile().GetTile() == TileRegistry.ZEBRA_CROSSING_1x1) {
            //TODO only switch states if they should actually cross here
            return typeof(ApproachZebraCrossingState);
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