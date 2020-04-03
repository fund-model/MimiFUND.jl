@defcomposite climatecomposite begin

    Component(climateco2cycle)
    Component(climatech4cycle)
    Component(climaten2ocycle)
    Component(climatesf6cycle)
    Component(climateforcing)
    Component(climatedynamics)
    Component(biodiversity)
    Component(climateregional)
    Component(ocean)

    mco2 = Parameter()
    globch4 = Parameter()
    globn2o = Parameter()
    globsf6 = Parameter()

    connect(climateco2cycle.mco2, mco2)
    connect(climatech4cycle.globch4, globch4)
    connect(climaten2ocycle.globn2o, globn2o)
    connect(climatesf6cycle.globsf6, globsf6)

    connect(climateco2cycle.temp, climatedynamics.temp)
    connect(climateforcing.acco2, climateco2cycle.acco2)
    connect(climateforcing.acch4, climatech4cycle.acch4)
    connect(climateforcing.acn2o, climaten2ocycle.acn2o)
    connect(climateforcing.acsf6, climatesf6cycle.acsf6)
    connect(climatedynamics.radforc, climateforcing.radforc)
    connect(climateregional.inputtemp, climatedynamics.temp)
    connect(biodiversity.temp, climatedynamics.temp)
    connect(ocean.temp, climatedynamics.temp)

    temp = Variable(climateregional.temp)
    acco2 = Variable(climateco2cycle.acco2)
    regtmp = Variable(climateregional.regtmp)
    regstmp = Variable(climateregional.regstmp)
    nospecies = Variable(biodiversity.nospecies)
    sea = Variable(ocean.sea)
end