# Fund.jl

This is an implementation of FUND in Julia. Everything is experimental at this point and this is not the official FUND code.

## Requirements

The minimum requirement to run FUND is [Julia](http://julialang.org/) and the [IAMF](https://bitbucket.org/davidanthoff/iamf.jl) package. To plot you should also install the [Winston](https://github.com/nolta/Winston.jl) package. To run the example IJulia notebook file you need to install [IJulia](https://github.com/JuliaLang/IJulia.jl).

### Installing Julia

You can download Julia from [http://julialang.org/downloads/](http://julialang.org/downloads/). You should use the v0.3.0-prerelease version and install it.

### Installing the IAMF package

Ask David Anthoff to give you access to the [IAMF repository](https://bitbucket.org/davidanthoff/iamf.jl) on bitbucket. Then start Julia and enter the following command on the Julia prompt:

````jl
Pkg.clone("git@bitbucket.org:davidanthoff/IAMF.jl.git")
````

### Installing Winston (optional)

[Winston](https://github.com/nolta/Winston.jl) is one of many Julia plotting packages. To install it, start Julia and enter the following command on the Julia prompt:

````jl
Pkg.add("Winston")
````

### Installing IJulia (optional)

[IJulia](https://github.com/JuliaLang/IJulia.jl) provides the excellent [IPython](http://ipython.org/) interface for the Julia language. Installation is a two step procedure:

First, you need a working IPython installation. The Anaconda distribution [(http://continuum.io/downloads)](http://continuum.io/downloads) is currently the easiest way to install IPython.

Second, start Julia, then enter the following command on the Julia prompt:

````jl
Pkg.add("IJulia")
````

To start the IJulia notebook, open a normal command prompt (not Julia), change to the directory with the FUND source code and enter the following command:

````
ipython notebook --profile julia
````


### Keeping requirements up-to-date (optional)

Many of these requirements are regularily updated. To make sure you have the latest versions, periodically execute the following command on the Julia prompt:

````jl
Pkg.update()
````
