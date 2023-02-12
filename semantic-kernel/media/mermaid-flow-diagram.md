flowchart LR
    A[ASK] --> B[Kernel]
    B --> C[Planner]
    C --> |Gather| E 
    C --> |Gather| F
    C --> |Gather| G
    E[Skills] --> |Inform| H
    F[Memories] --> |Inform| H 
    G[Connectors] --> |Inform| H
    H[Steps\nPlanned] --> |Pipeline| J[GET]