# FUND

## Overview

The Climate Framework for Uncertainty, Negotiation and Distribution (FUND) is a so-called integrated assessment model of climate change. FUND was originally set-up to study the role of international capital transfers in climate policy, but it soon evolved into a test-bed for studying impacts of climate change in a dynamic context, and it is now often used to perform cost-benefit and cost-effectiveness analyses of greenhouse gas emission reduction policies, to study equity of climate change and climate policy, and to support game-theoretic investigations into international environmental agreements.

FUND links scenarios and simple models of population, technology, economics, emissions, atmospheric chemistry, climate, sea level, and impacts. Together, these elements describe not-implausible futures. The model runs in time-steps of one year from 1950 to 2300, and distinguishes 16 major world regions.

FUND further includes the option to reduce emissions of industrial carbon dioxide. Reductions can be set by the user, or calculated so as to meet certain criteria set by the user.

An integrated assessment model, FUND is used to advice policymakers about proper and not-so-proper strategies. The model, however, always reflects its developer's world views. It is therefore regularly contrary to the rhetoric of politicians, and occasionally politically incorrect.

It is the developer's firm belief that most researchers should be locked away in an ivory tower. Models are often quite useless in unexperienced hands, and sometimes misleading. No one is smart enough to master in a short period what took someone else years to develop. Not-understood models are irrelevant, half-understood models treacherous, and mis-understood models dangerous.

Therefore, FUND does not have a pretty interface, and you will have to make to real effort to let it do something, let alone to let it do something new.

FUND was originally developed by Richard Tol. It is now co-developed by David Anthoff and Richard Tol. FUND does not have an institutional home.

## Getting Started

The minimum requirement to run FUND is [Julia](http://julialang.org/) and the [Mimi](https://github.com/davidanthoff/Mimi.jl) package. To run the example IJulia notebook file you need to install [IJulia](https://github.com/JuliaLang/IJulia.jl).

### Installing Julia

You can download Julia from [http://julialang.org/downloads/](http://julialang.org/downloads/). You should use the v0.5.x version and install it.

### Installing the Mimi package

Start Julia and enter the following command on the Julia prompt:

````julia
Pkg.add("Mimi")
````

### Keeping requirements up-to-date (optional)

Many of these requirements are regularly updated. To make sure you have the latest versions, periodically execute the following command on the Julia prompt:

````julia
Pkg.update()
````
