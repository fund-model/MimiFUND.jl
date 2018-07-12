using Mimi
using Distributions

mcs = @defmcs begin

    # Distributional parameters from data directory
    agel = Truncated(Normal(0.31,0.15), 0.0, 1.0)
    climatesensitivity = Truncated(Gamma(6.47815626,0.547629469), 1.0, Inf)
    # aceiadd[1] = Normal(0.0,0.025)
    # aceiadd[2] = Normal(0.0,0.025)
    # aceiadd[3] = Normal(0.0,0.025)
    # aceiadd[4] = Normal(0.0,0.0375)
    # aceiadd[5] = Normal(0.0,0.0375)
    # aceiadd[6] = Normal(0.0,0.025)
    # aceiadd[7] = Normal(0.0,0.025)
    # aceiadd[8] = Normal(0.0,0.0875)
    # aceiadd[9] = Normal(0.0,0.05625)
    # aceiadd[10] = Normal(0.0,0.05625)
    # aceiadd[11] = Normal(0.0,0.0875)
    # aceiadd[12] = Normal(0.0,0.0875)
    # aceiadd[13] = Normal(0.0,0.0375)
    # aceiadd[14] = Normal(0.0,0.11875)
    # aceiadd[15] = Normal(0.0,0.11875)
    # aceiadd[16] = Normal(0.0,0.05625)

    
    save(
        climatedynamics.temp,
        impactaggregation.loss
    )

end 
