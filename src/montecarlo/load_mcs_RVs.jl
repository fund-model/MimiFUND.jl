using DataStructures

txt_out = "src/montecarlo/mcs_RVs.txt"
datadir = "data"

files = readdir(datadir)
filter!(i->i!="desktop.ini", files)
parameters = OrderedDict{Any, Any}(splitext(file)[1] => readdlm(joinpath(datadir,file), ',') for file in files)

function convert(pv)
    if isa(pv,AbstractString)
        if startswith(pv,"~") & endswith(pv,")")
            args_start_index = search(pv,'(')
            dist_name = pv[2:args_start_index-1]
            args = split(pv[args_start_index+1:end-1], ';')
            fixedargs = filter(i->!contains(i,"="),args)
            optargs = Dict(split(i,'=')[1]=>split(i,'=')[2] for i in filter(i->contains(i,"="),args))

            if dist_name == "N"
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basenormal = "Normal($(parse(Float64, fixedargs[1])),$(parse(Float64, fixedargs[2])))"

                if length(optargs)==0
                    return basenormal
                else
                    min = haskey(optargs,"min") ? parse(Float64, optargs["min"]) : -Inf
                    max = haskey(optargs,"max") ? parse(Float64, optargs["max"]) : Inf
                    return "Truncated($basenormal, $min, $max)"
                end
            elseif startswith(pv, "~Gamma(")
                if length(fixedargs)!=2 error() end
                if length(optargs)>2 error() end

                basegamma = "Gamma($(parse(Float64, fixedargs[1])),$(parse(Float64, fixedargs[2])))"

                if length(optargs)==0
                    return basegamma
                else
                    min = haskey(optargs,"min") ? parse(Float64, optargs["min"]) : -Inf
                    max = haskey(optargs,"max") ? parse(Float64, optargs["max"]) : Inf
                    return "Truncated($basegamma, $min, $max)"
                end
            elseif startswith(pv, "~Triangular(")
                triang = "TriangularDist($(parse(Float64, fixedargs[1])), $(parse(Float64, fixedargs[2])), $(parse(Float64, fixedargs[3])))"
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
        return "$pv"
    else
        return nothing #"$pv"
    end
end

cat(x,y)=string(x,", ",y)
function prepparameter(p)
    column_count = size(p,2)
    if column_count == 1
        val = convert(p[1,1])
    elseif column_count == 2
        if isa(p[1,2], SubString)
            val = "[$(reduce(cat, [convert(p[j,2]) for j in 1:size(p,1)]))]"
        else 
            return nothing 
        end
    elseif column_count == 3
        if isa(p[1,3], SubString)
            length_index1 = length(unique(p[:,1]))
            length_index2 = length(unique(p[:,2]))
            new_p = Array{String}(length_index1,length_index2)
            cur_1 = 1
            cur_2 = 1
            for j in 1:size(p,1)
                new_p[cur_1,cur_2] = convert(p[j,3])
                cur_2 += 1
                if cur_2 > length_index2
                    cur_2 = 1
                    cur_1 += 1
                end
            end
            val = new_p
        else
            return nothing
        end
    end
    return val
end

f = open(txt_out, "w")

for (k,v) in parameters
    val = prepparameter(v)
    if val != nothing
        write(f, "$k = $val\n")
    end
end 

close(f)