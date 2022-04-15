using System;

public class CrossingState : PedestrianBaseState {

    public CrossingState(PedestrianAgent agent) {
        this.stateName = "Crossing State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        if (agent.GetCurrentTile().GetTile() != TileRegistry.ZEBRA_CROSSING_1x1) {
            return typeof(WalkState);
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