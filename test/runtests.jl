using Mimi

#general test
include("../src/fund.jl")
using fund

m = fund.FUND
run(m)

#test that v0.5.0 integration produces exactly the same results as v0.4.0
include("test_integration.jl")

