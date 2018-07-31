using Mimi

@defcomp impactsealevelrise begin
    regions = Index()

    wetval = Variable(index=[time,regions])
    wetlandloss = Variable(index=[time,regions])
    cumwetlandloss = Variable(index=[time,regions])
    wetlandgrowth = Variable(index=[time,regions])
    wetcost = Variable(index=[time,regions])


    dryval = Variable(index=[time,regions])
    landloss = Variable(index=[time,regions])
    cumlandloss = Variable(index=[time,regions])
    drycost = Variable(index=[time,regions])

    npprotcost = Variable(index=[time,regions])
    npwetcost = Variable(index=[time,regions])
    npdrycost = Variable(index=[time,regions])

    protlev = Variable(index=[time,regions])
    protcost = Variable(index=[time,regions])

    enter = Variable(index=[time,regions])
    leave = Variable(index=[time,regions])
    entercost = Variable(index=[time,regions])
    leavecost = Variable(index=[time,regions])

    imigrate = Variable(index=[regions,regions])

    incdens     = Parameter(default = 0.000635)
    emcst       = Parameter(default = 3)
    immcst      = Parameter(default = 0.4)
    dvydl       = Parameter(default = 1)
    wvel        = Parameter(default = 1.16)
    wvbm        = Parameter(default = 0.00588)
    slrwvpopdens0 = Parameter(default = 27.5937717888728)
    wvpdl       = Parameter(default = 0.47)
    wvsl        = Parameter(default = -0.11)
    dvbm        = Parameter(default = 0.004)
    slrwvypc0   = Parameter(default = 25000)

    pc          = Parameter(index=[regions], default = [95.3, 13.0, 153.9, 75.5, 36.6, 3.1, 54.0, 18.9, 42.3, 117.6, 172.0, 169.7, 118.4, 19.0, 84.3, 16.0])
    slrprtp     = Parameter(index=[regions], default = [0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.03])
    wmbm        = Parameter(index=[regions], default = [789.0, 0.0, 903.0, 7.0, 183.0, 0.0, 0.0, 0.0, 238.0, 4748.0, 0.0, 4.0, 1779.0, 0.0, 345.0, 169.0])
    dlbm        = Parameter(index=[regions], default = [20000.0, 970.0, 4212.0, 2687.0, 3135.0, 1889.0, 15138.0, 1621.0, 12004.0, 29407.0, 81275.0, 157286.0, 35000.0, 8354.0, 126602.0, 1505.0])
    drylandlossparam = Parameter(index=[regions], default = [0.583433160330257, 0.260515053189669, 0.273314149409615, 0.412066944374299, 0.548496484507909, 0.193491238230705, 0.555487676094765, 0.628422279667412, 0.678302864355753, 0.756134247750368, 0.929957316565567, 0.81223935947527, 0.707719142022655, 0.336949623686827, 0.798533208857799, 0.667255915065629])
    wlbm        = Parameter(index=[regions], default = [11400.0, 0.0, 3210.0, 573.0, 256.0, 38.0, 0.0, 0.0, 14775.0, 27234.0, 14303.0, 50885.0, 5879.0, 2649.0, 27847.0, 1528.0])
    coastpd     = Parameter(index=[regions], default = [1.04, 1.07, 4.48, 1.04, 1.27, 3.01, 0.75, 3.42, 0.69, 1.97, 2.31, 1.8, 4.46, 17.61, 9.69, 2.51])
    wetmax      = Parameter(index=[regions], default = [31049.0, 0.0, 37202.0, 3763.0, 2511.0, 5.0, 0.0, 140.0, 54279.0, 278791.0, 65483.0, 289431.0, 19132.0, 7928.0, 92617.0, 5606.0])
    wetland90   = Parameter(index=[regions], default = [42828.8, 130509.75, 95000.79, 4609.85, 55385.64, 11297.61, 118955.64, 16247.05, 76001.27, 394296.21, 74226.89, 299546.88, 31321.6, 9304.2, 236097.24, 6271.74])
    maxlandloss = Parameter(index=[regions], default = [1.373498e6, 1.170585e6, 1.004586e6, 171553.0, 1.514759e6, 220274.0, 5.527204e6, 601498.0, 509083.0, 3.131627e6, 1.241785e6, 1.908853e6, 973897.0, 445413.0, 1.395419e6, 206778.0])

    sea = Parameter(index=[time])

    migrate = Parameter(index=[regions,regions])    # has default, but too big to paster

    income = Parameter(index=[time,regions])
    population = Parameter(index=[time,regions])
    area = Parameter(index=[time,regions])

    function run_timestep(p, v, d, t)

        if is_first(t)
            for r1 in d.regions
                for r2 in d.regions
                    immsumm = 0
                    for i in d.regions
                        immsumm += p.migrate[i, r1]
                    end
                    v.imigrate[r1, r2] = p.migrate[r2, r1] / immsumm
                end

                t0 = 1
                v.landloss[t0, r1] = 0.0
                v.cumlandloss[t0, r1] = 0.0
                v.cumwetlandloss[t0, r1] = 0.0
                v.wetlandgrowth[t0, r1] = 0.0
            end
        else
            # slr in m/year
            ds = p.sea[t] - p.sea[t - 1]

            for r in d.regions
                ypc = p.income[t, r] / p.population[t, r] * 1000.0
                ypcprev = p.income[t - 1, r] / p.population[t - 1, r] * 1000.0
                ypcgrowth = ypc / ypcprev - 1.0

                if gettime(t) == 1951
                    ypcgrowth = 0
                end

                # Needs to be in $bn per km^2
                # Income is in billion, area is in km^2
                incomedens = p.income[t, r] / p.area[t, r]

                incomedensprev = p.income[t - 1, r] / p.area[t - 1, r]

                incomedensgrowth = incomedens / incomedensprev - 1.0

                # In population/km^2
                ## population is in million, area is in km^2
                popdens = p.population[t, r] / p.area[t, r] * 1000000.0
                popdensprev = p.population[t - 1, r] / p.area[t - 1, r] * 1000000.0
                popdensgrowth = popdens / popdensprev - 1.0

                # Unit of dryval is $bn/km^2
                v.dryval[t, r] = p.dvbm * (incomedens / p.incdens)^p.dvydl

                # Unit of wetval is $bn/km^2
                v.wetval[t, r] = p.wvbm *
                    (ypc / p.slrwvypc0)^p.wvel *
                    (popdens / p.slrwvpopdens0)^p.wvpdl *
                    ((p.wetland90[r] - v.cumwetlandloss[t - 1, r]) / p.wetland90[r])^p.wvsl

                potCumLandloss = min(p.maxlandloss[r], p.dlbm[r] * p.sea[t]^p.drylandlossparam[r])

                potLandloss = potCumLandloss - v.cumlandloss[t - 1, r]

                ## If sea levels fall, no protection is build
                if ds < 0
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 0
                # If the discount rate < -100% people will not build protection
                elseif (1.0 + p.slrprtp[r] + ypcgrowth) < 0.0
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 0
                # Dryland value is worthless
                elseif (1.0 + p.dvydl * incomedensgrowth) < 0.0
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 0
                # Is protecting the coast infinitly expensive?
                elseif (1.0 / (1.0 + p.slrprtp[r] + ypcgrowth)) >= 1
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 0
                # Is dryland infinitly valuable?
                elseif ((1.0 + p.dvydl * incomedensgrowth) / (1.0 + p.slrprtp[r] + ypcgrowth)) >= 1.0
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 1
                # Is wetland infinitly valuable?
                elseif ((1.0 + p.wvel * ypcgrowth + p.wvpdl * popdensgrowth + p.wvsl * v.wetlandgrowth[t - 1, r]) / (1.0 + p.slrprtp[r] + ypcgrowth)) >= 1.0
                    v.npprotcost[t, r] = 0
                    v.npwetcost[t, r] = 0
                    v.npdrycost[t, r] = 0
                    v.protlev[t, r] = 0
                else
                    # NPV of protecting the whole coast
                    # pc is in $bn/m
                    v.npprotcost[t, r] = p.pc[r] * ds * (1.0 + p.slrprtp[r] + ypcgrowth) / (p.slrprtp[r] + ypcgrowth)

                    # NPV of wetland
                    if (1.0 + p.wvel * ypcgrowth + p.wvpdl * popdensgrowth + p.wvsl * v.wetlandgrowth[t - 1, r]) < 0.0
                        v.npwetcost[t, r] = 0
                    else
                        v.npwetcost[t, r] = p.wmbm[r] * ds * v.wetval[t, r] * (1.0 + p.slrprtp[r] + ypcgrowth) / (p.slrprtp[r] + ypcgrowth - p.wvel * ypcgrowth - p.wvpdl * popdensgrowth - p.wvsl * v.wetlandgrowth[t - 1, r])
                    end

                    # NPV of dryland
                    if (1.0 + p.dvydl * incomedensgrowth) < 0.0
                        v.npdrycost[t, r] = 0
                    else
                        v.npdrycost[t, r] = potLandloss * v.dryval[t, r] * (1 + p.slrprtp[r] + ypcgrowth) / (p.slrprtp[r] + ypcgrowth - p.dvydl * incomedensgrowth)
                    end

                    # Calculate protection level
                    v.protlev[t, r] = max(0.0, 1.0 - 0.5 * (v.npprotcost[t, r] + v.npwetcost[t, r]) / v.npdrycost[t, r])

                    if v.protlev[t, r] > 1
                        error("protlevel >1 should not happen")
                    end
                end

                # Calculate actual wetland loss and cost
                v.wetlandloss[t, r] = min(
                    p.wlbm[r] * ds + v.protlev[t, r] * p.wmbm[r] * ds,
                    p.wetmax[r] - v.cumwetlandloss[t - 1, r])

                v.cumwetlandloss[t, r] = v.cumwetlandloss[t - 1, r] + v.wetlandloss[t, r]

                # Calculate wetland growth
                v.wetlandgrowth[t, r] = (p.wetland90[r] - v.cumwetlandloss[t, r]) / (p.wetland90[r] - v.cumwetlandloss[t - 1, r]) - 1.0

                v.wetcost[t, r] = v.wetval[t, r] * v.wetlandloss[t, r]

                v.landloss[t, r] = (1.0 - v.protlev[t, r]) * potLandloss

                v.cumlandloss[t, r] = v.cumlandloss[t - 1, r] + v.landloss[t, r]
                v.drycost[t, r] = v.dryval[t, r] * v.landloss[t, r]

                v.protcost[t, r] = v.protlev[t, r] * p.pc[r] * ds

                if v.landloss[t, r] < 0
                    v.leave[t, r] = 0
                else
                    v.leave[t, r] = p.coastpd[r] * popdens * v.landloss[t, r]
                end

                v.leavecost[t, r] = p.emcst * ypc * v.leave[t, r] / 1000000000
            end

            for destination in d.regions
                enter = 0.0
                for source in d.regions
                    enter += v.leave[t, source] * v.imigrate[source, destination]
                end
                v.enter[t, destination] = enter
            end

            for r in d.regions
                ypc = p.income[t, r] / p.population[t, r] * 1000.0
                v.entercost[t, r] = p.immcst * ypc * v.enter[t, r] / 1000000000
            end
        end
    end
end
