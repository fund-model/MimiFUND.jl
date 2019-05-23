# FUND

[![Build Status](https://travis-ci.org/fund-model/MimiFUND.jl.svg?branch=master)](https://travis-ci.org/fund-model/MimiFUND.jl)

## Overview

The Climate Framework for Uncertainty, Negotiation and Distribution (FUND) is a so-called integrated assessment model of climate change. FUND was originally set-up to study the role of international capital transfers in climate policy, but it soon evolved into a test-bed for studying impacts of climate change in a dynamic context, and it is now often used to perform cost-benefit and cost-effectiveness analyses of greenhouse gas emission reduction policies, to study equity of climate change and climate policy, and to support game-theoretic investigations into international environmental agreements.

FUND links scenarios and simple models of population, technology, economics, emissions, atmospheric chemistry, climate, sea level, and impacts. Together, these elements describe not-implausible futures. The model runs in time-steps of one year from 1950 to 2300, and distinguishes 16 major world regions.

FUND further includes the option to reduce emissions of industrial carbon dioxide. Reductions can be set by the user, or calculated so as to meet certain criteria set by the user.

An integrated assessment model, FUND is used to advice policymakers about proper and not-so-proper strategies. The model, however, always reflects its developer's world views. It is therefore regularly contrary to the rhetoric of politicians, and occasionally politically incorrect.

It is the developer's firm belief that most researchers should be locked away in an ivory tower. Models are often quite useless in unexperienced hands, and sometimes misleading. No one is smart enough to master in a short period what took someone else years to develop. Not-understood models are irrelevant, half-understood models treacherous, and mis-understood models dangerous.

Therefore, FUND does not have a pretty interface, and you will have to make to real effort to let it do something, let alone to let it do something new.

FUND was originally developed by Richard Tol. It is now co-developed by David Anthoff and Richard Tol. FUND does not have an institutional home.

## Getting Started

The minimum requirement to run FUND is [Julia](http://julialang.org/) and the [Mimi](https://github.com/mimiframework/Mimi.jl) package. To run the example IJulia notebook file you need to install [IJulia](https://github.com/JuliaLang/IJulia.jl).

### Software Requirements

You need to install [Julia 1.1.0](https://julialang.org) or newer to run this model.  You can download Julia from [http://julialang.org/downloads/](http://julialang.org/downloads/).

### Preparing the Software Environment

You first need to connect your julia installation with the central Mimi registry of Mimi models. This central registry is like a catalogue of models that use Mimi that is maintained by the Mimi project. To add this registry, run the following command at the julia package REPL:

```julia
pkg> registry add https://github.com/mimiframework/MimiRegistry.git
You only need to run this command once on a computer.
```

The next step is to install MimiFUND.jl itself. You need to run the following command at the julia package REPL:

```julia
pkg> add MimiFUND
```

You probably also want to install the Mimi package into your julia environment, so that you can use some of the tools in there:

```
pkg> add Mimi
```

### Keeping requirements up-to-date (optional)

Many of these requirements are regularly updated. To make sure you have the latest versions, periodically execute the following command on the Julia prompt:

```julia
pkg> up
```
## Running the model

The model uses the Mimi framework and it is highly recommended to read the Mimi documentation first to understand the code structure. For starter code on running the model just once, see the code in the file `examples/main.jl`.

The basic way of accessing a copy of the default MimiFUND model is the following:
```
using MimiFUND

m = MimiFUND.get_model()
run(m)
```
## Calculating the Social Cost of Carbon

Here is an example of computing the social cost of carbon with MimiFUND. Note that the units of the returned value are $/ton CO2.

```
using Mimi
using MimiFUND

# Get the social cost of carbon in year 2020 from the default MimiFUND model:
scc = MimiFUND.compute_scc(year = 2020)

# Or, you can also compute the SCC from a modified version of a MimiFUND model:
m = MimiFUND.get_model() # Get the default version of the FUND model
update_param!(m, :climatesensitivity, 5) # make any modifications to your model
scc = MimiFUND.compute_scc(m, year = 2020) # Compute the SCC from your model
```

There are several keyword arguments available to `compute_scc`. Note that the user must specify a `year` for the SCC calculation, but the rest of the keyword arguments have default values.
```
compute_scc(m = get_model(),  # if no model provided, will use the default MimiFUND model
    year = nothing,  # user must specify an emission year for the SCC calculation
    gas = :CO2,  # which greenhouse gas to use. Other options are :CH4, :N2O, and :SF6.
    last_year = 3000,  # the last year to run and use for the SCC calculation. Default is the last year of the time dimension, 3000.
    eta = 1.45,  # eta parameter for ramsey discounting representing the elasticity of marginal utility
    prtp = 0.015,  # pure rate of time preference parameter for discounting
    equity_weights = false  # whether or not to use regional equity weighting
)
```

There is an additional function for computing the SCC that also returns the MarginalModel that was used to compute it. It returns these two values as a NamedTuple of the form (scc=scc, mm=mm). The same keyword arguments from the `compute_scc` function are available for the `compute_scc_mm` function. Example:
```
using Mimi
using MimiFUND

result = compute_scc_mm(year=2020, last_year=2300, eta=0, prtp=0.03)

result.scc  # returns the computed SCC value

result.mm   # returns the Mimi MarginalModel

marginal_temp = result.mm[:climatedynamics, :temp]  # marginal results from the marginal model can be accessed like this
```

## Versions and academic use policy

Released versions of FUND have a git tag in this repository *and* the ``master`` branch either points to that version, or a newer version. In general we increase at least the minor part of the version (the versions follow the ``major.minor.patch`` pattern) whenever we change any of the equations or calibrations. All versions with a git tag that is at least as new as the git tag that ``master`` points to have been used in at least one publication and we welcome if other researchers use those versions for their own work and in their publications.

The ``master`` branch in this repository always points to the latest released versions, i.e. it will always point to a version that has a git tag and is released.

The ``next`` branch (and any git tags that are newer than the git tag that ``master`` points to) contains work in progress. In general you should not assume that the ``next`` branch is ready for use for anything. It often is in an intermediate state between released versions, where we have started changes that are not finished etc. While the code on the ``next`` branch is technically licensed under the MIT license, we kindly ask other researchers to not publish papers based on versions that they can see in the ``next`` branch. The versions in that branch represent ongoing work by us that we haven't gotten academic credit for because we have not yet published something with these versions, and we therefore ask other researchers to not use those versions on their own. You can of course always approach us about joint work when it comes to the version on the ``next`` branch and then we can discuss how we handle that.
