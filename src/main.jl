using Mimi

include("fund.jl")
using .Fund

m = getfund()
run(m)

explore(m)