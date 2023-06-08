# FUND

![](https://github.com/fund-model/MimiFUND.jl/actions/workflows/jlpkgbutler-ci-master-workflow.yml/badge.svg)
[![Project Status: Active - The project has reached a stable, usable state and is being actively developed.](http://www.repostatus.org/badges/latest/active.svg)](http://www.repostatus.org/#active)
[![codecov](https://codecov.io/gh/fund-model/MimiFUND.jl/branch/master/graph/badge.svg)](https://codecov.io/gh/fund-model/MimiFUND.jl)

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

You need to install [Julia 1.6.0](https://julialang.org) or newer to run this model.  You can download Julia from [http://julialang.org/downloads/](http://julialang.org/downloads/).

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

```julia
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
```julia
using MimiFUND

m = MimiFUND.get_model()
run(m)
```
## Calculating the Social Cost of CO2 and other gases

Here is an example of computing the Social Cost of CO2 with MimiFUND. Note that the units of the returned value are 1995$ per metric tonne of CO2.

```julia
using Mimi
using MimiFUND

# Get the Social Cost of CO2 in year 2020 from the default MimiFUND model:
scc = MimiFUND.compute_scco2(year = 2020, eta = 0., prtp = 0.03, equity_weights = false)

# Or, you can also compute the SC-CO2 from a modified version of a MimiFUND model:
m = MimiFUND.get_model()                        # Get the default version of the FUND model
update_param!(m, :climatedynamics, :climatesensitivity, 5)        # make any modifications to your model using Mimi
scc = MimiFUND.compute_scco2(m, year = 2020)    # Compute the SC-CO2 from your model
```
There are also functions for computing the Social Cost of CH4, N2O, and SF6: `compute_scch4`, `compute_scn2o`, and `compute_scsf6`.
These functions are all wrappers for the generic social cost function `compute_sc`, which takes a keyword `gas` with default value `:CO2`.

There are several other keyword arguments available to `compute_sc`. Note that the user must specify a `year` for the SC calculation, 
but the rest of the keyword arguments have default values.
```julia
MimiFUND.compute_sc(m::Model=get_model();
        gas::Symbol = :CO2,                     
        year::Union{Int, Nothing} = nothing,    
        eta::Float64 = 1.45,                    
        prtp::Float64 = 0.015,                  
        equity_weights::Bool = false,   
        equity_weights_normalization_region::Int = 0,        
        last_year::Int = 3000,                  
        pulse_size::Float64 = 1e7,              
        return_mm::Bool = false,
        n::Union{Int, Nothing} = nothing,
        trials_output_filename::Union{String, Nothing} = nothing,
        seed::Union{Int, Nothing} = nothing)
```
Description of keyword arguments:
- **`m`**: a MimiFUND model from which to calculate the social cost. If no model is provided, the default MimiFUND model will be used. Note that the provided model `m` can be a highly modified MimiFUND model, but certain internal structures of the model need to remain in order for the `compute_sc` function to work. They are:
    - The original parameter connection between the `emissions` component and the climate cycling component for the specified `gas` must still be intact (this is where the pulse of emissions is added).
    - There must still be a `:socioeconomic` component with fields `:ypc` and `:globalypc` (used for discounting).
    - There must still be an `:impactaggregation` component with field `:loss`, which is the total damages value from which the social cost is calculated.
- **`gas`**: which greenhouse gas to calculate the social cost for. The default is `:CO2`. Other options are `:CH4`, `:N2O`, and `:SF6`.
- **`year`**: the user must specify an emission year for the SC calculation. Valid years are 1951 to 2990.
- **`eta`**: the elasticity of marginal utility to be used in ramsey discounting. Setting `eta = 0` is equivalent to constant discounting with rate `prtp`.
- **`prtp`**: pure rate of time preference parameter for discounting
- **`equity_weights`**: whether or not to use regional equity weighting in discounting
- **`equity_weights_normalization_region`**: normalization region for equity weighting (0=world average, any other value specifies the region index)
- **`last_year`**: the last year to run and use for the SC calculation. Default is the last year of FUND's time index, 3000.
- **`pulse_size`**: the size of the marginal emissions pulse, in metric tonnes of the specified `gas`. Changing this value will not change the units of the returned value, which are always "1995$ per metric tonne of `gas`". The returned value is always normalized by the size of `pulse_size` that is used.
- **`return_mm`**: whether or not to also return the MarginalModel used in the social cost calculation. If set to `true`, then a NamedTuple `(sc = sc, mm = mm)` of the social cost value and the MarginalModel used to compute it is returned.
- **`n`**: By default, `n = nothing`, and a single value for the "best guess" social cost is returned. If a positive value for keyword `n` is specified, then a Monte Carlo simulation with sample size `n` will run, sampling from all of FUND's random variables, and a vector of `n` social cost values will be returned. Note that if the user has provided a modified model `m`, the user modifications may be overwritten by the Monte Carlo simulation. If the user has modified certain parameter values, but they are parameters that have assigned random variable distributions in FUND, then they will be overwritten by the sampled values. For a list of which parameters have assigned random variable definitions, see "src/montecarlo/defmcs.jl"
- **`trials_output_filename`**: an optional CSV file path to save all of the sampled trial data.
- **`seed`**: the user can optionally provide a seed value, which will set the random seed before the simulation is run. This allows results to be replicated. 


Example Monte Carlo simulation:
```julia
using Mimi
using MimiFUND
using Statistics

scco2_values = MimiFUND.compute_sc(year = 2020, gas = :CO2, eta = 1.0, prtp = 0.01, n = 1000)
mean(scco2_values)
median(scco2_values)

# Experiment with the same set of trial data by setting the seed (any Integer value)
values_lo_discounting = MimiFUND.compute_sc(year = 2020, gas = :CO2, eta = 1., prtp = 0.015, n = 1000, seed = 999)
values_hi_discounting = MimiFUND.compute_sc(year = 2020, gas = :CO2, eta = 1., prtp = 0.05, n = 1000, seed = 999)
```

Example of working with the MarginalModel from setting `return_mm = true`:
```julia
using Mimi
using MimiFUND

result = MimiFUND.compute_sc(year = 2020, gas = :CH4, last_year = 2300, eta = 0, prtp = 0.03, return_mm = true)

result.sc  # returns the computed SC-CH4 value

result.mm   # returns the Mimi MarginalModel

marginal_temp = result.mm[:climatedynamics, :temp]  # marginal results from the marginal model can be accessed like this
```

### Pulse Size Details

By default, MimiFUND will calculate the SC using a marginal emissions pulse of 10 GtCO2 spread over ten years, or 1 GtCO2 (or Gt other gas) per year for ten years.  The SC will be always be returned in units of dollars per ton because it is normalized by the pulse size.  This choice of pulse size and duration is a decision made based on experiments with stability of results and moving from continuous to discretized equations, and can be found described further in the literature around FUND.

If you wish to alter this pulse size, it is an optional keyword argument to the  `compute_sc` function where `pulse_size` controls the size of the marginal emission pulse. For a deeper dive into the machinery of this function, see the forum conversation [here](https://forum.mimiframework.org/t/mimifund-emissions-pulse/153/9) and the docstrings in `new_marginaldamage.jl`.

## Versions and academic use policy

Released versions of FUND have a git tag in this repository *and* the ``master`` branch either points to that version, or a newer version. In general we increase at least the minor part of the version (the versions follow the ``major.minor.patch`` pattern) whenever we change any of the equations or calibrations. All versions with a git tag that is at least as new as the git tag that ``master`` points to have been used in at least one publication and we welcome if other researchers use those versions for their own work and in their publications.

The ``master`` branch in this repository always points to the latest released versions, i.e. it will always point to a version that has a git tag and is released.

The ``next`` branch (and any git tags that are newer than the git tag that ``master`` points to) contains work in progress. In general you should not assume that the ``next`` branch is ready for use for anything. It often is in an intermediate state between released versions, where we have started changes that are not finished etc. While the code on the ``next`` branch is technically licensed under the MIT license, we kindly ask other researchers to not publish papers based on versions that they can see in the ``next`` branch. The versions in that branch represent ongoing work by us that we haven't gotten academic credit for because we have not yet published something with these versions, and we therefore ask other researchers to not use those versions on their own. You can of course always approach us about joint work when it comes to the version on the ``next`` branch and then we can discuss how we handle that.

