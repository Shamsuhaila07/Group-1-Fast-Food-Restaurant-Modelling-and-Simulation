public enum AgentState
{
    // not all AgentState is set as some introduced some delay in the simulation
    // to mitigate the delay issues some AgentState is not set in the Agent script
    InQueue,
    ToQueue,
    Wandering,
    Peeking,
    Thinking,
    Ordering,
    EnteringKioskQueue,
    GettingOrder,
    EnteringCollectOrderQueue,
    Eating,
    Exiting,
}
