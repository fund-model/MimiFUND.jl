using Distributions

function loadparameters(datadir=joinpath(dirname(@__FILE__), "..", "data"))
    files = readdir(datadir)
    filter!(i->i!="desktop.ini", files)
    parameters = Dict{Any, Any}(splitext(file)[1] => readdlm(joinpath(datadir,file), ',', comments = true) for file in files)

    prepparameters!(parameters)

    return parameters
end

import StatsBase.mode
function mode(d::Truncated{Gamma{Float64},Continuous})
    return mode(d.untruncated)
end

function getindexfromyear(year)
    baseyear = 1950
    return year - baseyear + 1
end

function convertparametervalue(pv)
    if isa(pv,AbstractString)
        if startswith(pv,"~") & endswith(pv,")")
            args_start_index = something(findfirst(isequal('('), pv), 0) 
            dist_name = pv[2:args_start_index-1]
            args = split(pv[args_start_index+1:end-1], ';')
            fixedargs = filter(i->!occursin("=", i),args)
            optargs = Dict(split(i,'=')[1]=>split(i,'=')[2] for i in filter(i->occursin("=", i),args))

            if dist_name == "N"
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basenormal = Normal(parse(Float64, fixedargs[1]),parse(Float64, fixedargs[2]))

                if length(optargs)==0
                    return basenormal
                else
                    return Truncated(basenormal,
                        haskey(optargs,"min") ? parse(Float64, optargs["min"]) : -Inf,
                        haskey(optargs,"max") ? parse(Float64, optargs["max"]) : Inf)
                end
            elseif startswith(pv, "~Gamma(")
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basegamma = Gamma(parse(Float64, fixedargs[1]),parse(Float64, fixedargs[2]))

                if length(optargs)==0
                    return basegamma
                else
                    return Truncated(basegamma,
                        haskey(optargs,"min") ? parse(Float64, optargs["min"]) : -Inf,
                        haskey(optargs,"max") ? parse(Float64, optargs["max"]) : Inf)
                end
            elseif startswith(pv, "~Triangular(")
                triang = TriangularDist(parse(Float64, fixedargs[1]), parse(Float64, fixedargs[2]), parse(Float64, fixedargs[3]))
                return triang
            else
                error("Unknown distribution")
            end
        elseif pv=="true"
            return true
        elseif pv=="false"
            return false
        elseif endswith(pv, "y")
            return parse(Int, strip(pv,'y'))
        else
            try
                return parse(Float64, pv)
            catch e
                error(pv)
            end
        end
        return pv
    else
        return pv
    end
end

function getbestguess(p)
    if isa(p, ContinuousUnivariateDistribution)
        return mode(p)
    else
        return p
    end
end

function prepparameters!(parameters)
    for i in parameters
        p = i[2]
        column_count = size(p,2)
        if column_count == 1
            parameters[i[1]] = getbestguess(convertparametervalue(p[1,1]))
        elseif column_count == 2
            parameters[i[1]] = Float64[getbestguess(convertparametervalue(p[j,2])) for j in 1:size(p,1)]
        elseif column_count == 3
            length_index1 = length(unique(p[:,1]))
            length_index2 = length(unique(p[:,2]))
            new_p = Array{Float64}(undef, length_index1,length_index2)
            cur_1 = 1
            cur_2 = 1
            for j in 1:size(p,1)
                new_p[cur_1,cur_2] = getbestguess(convertparametervalue(p[j,3]))
                cur_2 += 1
                if cur_2 > length_index2
                    cur_2 = 1
                    cur_1 += 1
                end
            end
            parameters[i[1]] = new_p
        end
    end
end
