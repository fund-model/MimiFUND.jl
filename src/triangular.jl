# This is a temporary fix until the Distributions.jl package has a triangular distribution

using Distributions

immutable TriangularDist <: ContinuousUnivariateDistribution
    a::Float64
    b::Float64
    c::Float64
    function TriangularDist(a::Real, b::Real, c::Real)
        a < b || error("a<b must be true")
        a <= c <= b || error("a<=c<=b must be true")
        new(float64(a), float64(b), float64(c))
    end
end

import StatsBase.mode
mode(d::TriangularDist) = d.c

import Base.rand
function rand(d::TriangularDist)
    u = rand()

    if u < (d.c-d.a)/(d.b-d.a)
        return d.a + sqrt(u*(d.b-d.a)*(d.c-d.a))
    else
        return d.b - sqrt((1.0-u)*(d.b-d.a)*(d.b-d.c))
    end
end
