include("../src/fund.jl")
m = getfund(datadir=joinpath(dirname(@__FILE__),"..","data"))
run(m)
