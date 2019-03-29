@defcomp climateso2cycle begin
    acso2 = Variable(index=[time])
    globso2 = Parameter(index=[time])

    function run_timestep(p, v, d, t)
        if t.t > 1
            v.acso2[t] = p.globso2[t]
        end 
    end
end