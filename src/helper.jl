using Distributions
include("triangular.jl")

function loadparameters(datadir="../data")
    files = readdir(datadir)
    parameters = {lowercase(splitext(file)[1]) => readdlm(joinpath(datadir,file), ',') for file in files};

    prepparameters!(parameters)

    return parameters
end

import StatsBase.modes
function modes(d::Truncated{Gamma})
    return [mode(d.untruncated)]
end

function getindexfromyear(year)
    const baseyear = 1950
    return year - baseyear + 1
end

function convertparametervalue(pv)
    if !isa(pv,Float64)
        if beginswith(pv,"~") & endswith(pv,")")
            args_start_index = search(pv,'(')
            dist_name = pv[2:args_start_index-1]
            args = split(pv[args_start_index+1:end-1], ';')
            fixedargs = filter(i->!contains(i,"="),args)
            optargs = {split(i,'=')[1]=>split(i,'=')[2] for i in filter(i->contains(i,"="),args)}

            if dist_name == "N"
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basenormal = Normal(float64(fixedargs[1]),float64(fixedargs[2]))

                if length(optargs)==0
                    return basenormal
                else
                    return Truncated(basenormal,
                        haskey(optargs,"min") ? float64(optargs["min"]) : -Inf,
                        haskey(optargs,"max") ? float64(optargs["max"]) : Inf)
                end
            elseif beginswith(pv, "~Gamma(")
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basegamma = Gamma(float64(fixedargs[1]),float64(fixedargs[2]))

                if length(optargs)==0
                    return basegamma
                else
                    return Truncated(basegamma,
                        haskey(optargs,"min") ? float64(optargs["min"]) : -Inf,
                        haskey(optargs,"max") ? float64(optargs["max"]) : Inf)
                end
            elseif beginswith(pv, "~Triangular(")
                triang = TriangularDist(float64(fixedargs[1]), float64(fixedargs[2]), float64(fixedargs[3]))
                return triang
            else
                error("Unknown distribution")
            end
        elseif endswith(pv, "y")
            return int64(strip(pv,'y'))
        else
            error(pv)
        end
        return pv
    else
        return pv
    end
end

function getbestguess(p)
    if isa(p, ContinuousUnivariateDistribution) then
        return modes(p)[1]
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
            new_p = Array(Float64,length_index1,length_index2)
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
