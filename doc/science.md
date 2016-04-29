1. Resolution
=============

FUND 3.9 is defined for 16 regions, specified in Table R. The model runs
from 1950 to 3000 in time-steps of a year.

2. Population and income
========================

Population and per capita income follow exogenous scenarios. There are
five standard scenarios, specified in Tables P and Y. The FUND scenario
is based on the EMF14 Standardised Scenario, and lies somewhere in
between the IS92a and IS92f scenarios (Leggett *et al.,* 1992). The
other scenarios follow the SRES A1B, A2, B1 and B2 scenarios
(Nakicenovic and Swart, 2001), as implemented in the IMAGE model (IMAGE
Team, 2001).

We assume that all regions are in a steady state after the year 2300.
For the years 2301-3000 per capita income growth rates are constant and
equal to the values of the year 2300, while population does not change.

3. Emission, abatement and costs
================================

3.1. Carbon dioxide (CO~2~)
-------------------------

Carbon dioxide emissions are calculated on the basis of the Kaya
identity:

$$M_{t,r}=\frac{M_{t,r}}{E_{t,r}}\frac{E_{t,r}}{Y_{t,r}}\frac{Y_{t,r}}{P_{t,r}}P_{t,r}=\psi_{t,r}\varphi_{t,r}Y_{t,r}$$ {#eq:CO2_1 tag="CO1.1"}

where $M$ denotes emissions, $E$ denote energy use, $Y$ denotes GDP and
$P$ denotes population; $t$ is the index for time, $r$ for region. The
carbon intensity of energy use, and the energy intensity of production
follow from:

$$\psi_{t,r} = g_{t - 1,r}^{\psi}\psi_{t - 1,r} - \alpha_{t - 1,r}\tau_{t - 1,r}$$ {#eq:CO2_2 tag="CO2.2"}

and

$$\varphi_{t,r} = g_{t - 1,r}^{\varphi}\varphi_{t - 1,r} - \alpha_{t - 1,r}\tau_{t - 1,r}$$ {#eq:CO2_3 tag="CO2.3"}

where $\tau$ is policy intervention and $\alpha$ is a parameter. The
exogenous growth rates $g$ are referred to as the Autonomous Energy
Efficiency Improvement (AEEI) and the Autonomous Carbon Efficiency
Improvement (ACEI). See Tables AEEI and ACEI for the five alternative
scenarios (values for the years 2301-3000 again equal the values for the
year 2300). Policy also affects emissions via

$$M_{t,r} = \left( \psi_{t,r} - \chi_{t,r}^{\psi} \right)\left( \varphi_{t,r} - \chi_{t,r}^{\varphi} \right)Y_{t,r}$$ {#eq:CO2_1b tag="CO2.1'"}

$$\chi_{t,r}^{\psi} = \kappa_{\psi}\chi_{t - 1,r} + \left( 1 - \alpha_{t - 1,r} \right)\tau_{t - 1,r}^{\psi}$$ {#eq:CO2_4 tag="CO2.4"}

and

$$\chi_{t,r}^{\varphi} = \kappa_{\varphi}\chi_{t - 1,r} + \left( 1 - \alpha_{t - 1,r} \right)\tau_{t - 1,r}^{\varphi}$$ {#eq:CO2_5 tag="CO2.5"}

Thus, the variable $0 < \alpha < 1$ governs which part of emission
reduction is *permanent* (reducing carbon and energy intensities at all
future times) and which part of emission reduction is *temporary*
(reducing current energy consumptions and carbon emissions), fading at a
rate of $0 < \kappa < 1$. In the base case,
$\kappa_{\psi} = \kappa_{\varphi} = 0.9$ and

$$\alpha_{t,r} = 1 - \frac{\tau_{t,r}/100}{1 + \tau_{t,r}/100}$$ {#eq:CO2_6 tag="CO2.6"}

So that $\alpha = 0.5$ if $\tau = \$100/\text{tC}$. One may
interpret the difference between permanent and temporary emission
reduction as affecting commercial technologies and capital stocks,
respectively. The emission reduction module is a reduced form way of
modelling that part of the emission reduction fades away after the
policy intervention is reversed, but that another part remains through
technological lock-in. Learning effects are described below. The
parameters of the model are chosen so that *FUND* roughly resembles the
behaviour of other models, particularly those of the Energy Modeling
Forum (Weyant, 2004; Weyant *et al.*, 2006).

The costs of emission reduction $C$ are given by
$$\frac{C_{t,r}}{Y_{t,r}} = \frac{\beta_{t,r}\tau_{t,r}^{2}}{H_{t,r}H_{t}^{g}}$$ {#eq:CO2_7 tag="CO2.7"}

$H$ denotes the stock of knowledge. Equation (@eq:CO2_6) gives the costs of
emission reduction in a particular year for emission reduction in that
year. In combination with Equations (@eq:CO2_2)-(@eq:CO2_5), emission reduction
is cheaper if smeared out over a longer time period. The parameter
$\beta$ follows from

$$\beta_{t,r} = 0.784 - 0.084\sqrt{\frac{M_{t,r}}{Y_{t,r}} - \operatorname{}\frac{M_{t,s}}{Y_{t,s}}}$$ {#eq:CO2_8 tag="CO2.8"}

That is, emission reduction is relatively expensive for the region that
has the lowest emission intensity. The calibration is such that a 10%
emission reduction cut in 2003 would cost 1.57% (1.38%) of GDP of the
least (most) carbon-intensive region; this is calibrated to Hourcade *et
al.* (1996, 2001). An 80% (85%) emission reduction would completely ruin
the economy. Later emission reductions are cheaper by Equations (@eq:CO2_7)
and (CO2.8). Emission reduction is relatively cheap for regions with
high emission intensities. The thought is that emission reduction is
cheap in countries that use a lot of energy and rely heavily on fossil
fuels, while other countries use less energy and less fossil fuels and
are therefore closer to the technological frontier of emission
abatement. For relatively small emission reduction, the costs in *FUND*
correspond closely to those reported by other top-down models, but for
higher emission reduction, *FUND* finds higher costs, because *FUND*
does not include backstop technologies, that is, a carbon-free energy
supply that is available in unlimited quantities at fixed average costs.

The regional and global knowledge stocks follow from

$$H_{t,r} = H_{t - 1,r}\sqrt{1 + \gamma_{R}\tau_{t - 1,r}}$$ {#eq:CO2_9 tag="CO2.9"}

and

$$H_{t}^{G} = H_{t - 1}^{G}\sqrt{1 + \gamma_{G}\tau_{t,r}}$$ {#eq:CO2_10 tag="CO2.10"}

Knowledge accumulates with emission abatement. More knowledge implies
lower emission reduction costs. The parameters $\gamma$ determine which
part of the knowledge is kept within the region, and which part spills
over to other regions as well. In the base case, $\gamma_{R} = 0.9$ and
$\gamma_{G} = 0.1$. The model is similar in structure and numbers to
that of Goulder and Schneider (1999) and Goulder and Mathai (2000).

Emissions from land use change and deforestation are exogenous, and
cannot be mitigated. Numbers are found in Tables CO2F, again for five
alternative scenarios.

3.2. Methane (CH~4~)
----------------------------------------------------------------------------------------------------------

Methane emissions are exogenous, specified in Table CH4 (emissions for
the years 2301-3000 are equal to emissions in the year 2300). There is a
single scenario only, based on IS92a (Leggett *et al.*, 1992). The costs
of emission reduction are quadratic. Table OC specifies the parameters,
which are calibrated to USEPA (2003).

3.3. Nitrous oxide (N~2~O)
--------------------------

Nitrous oxide emissions are exogenous, specified in Table N2O (emissions
for the years 2301-3000 are equal to emissions in the year 2300). There
is a single scenario only, based on IS92a (Leggett *et al.*, 1992). The
costs of emission reduction are quadratic. Table OC specifies the
parameters, which are calibrated to USEPA (2003).

3.4. Sulfurhexafluoride (SF~6~)
-------------------------------

SF~6~ emissions are linear in GDP and GDP per capita. Table SF6 gives
the parameters. The numbers for 1990 and 1995 are estimated from IEA
data (http://data.iea.org/ieastore/product.asp?dept\_id=101&pf\_id=305).
There is no option to reduce SF~6~ emissions.

3.5. Dynamic Biosphere
----------------------

Emissions from the terrestrial biosphere follow

$$E_{t}^{B} = \beta\left( T_{t} - T_{2010} \right)\frac{B_{t}}{B_{\mathrm{\max}}}$$ {#eq:DB_1 tag="DB.1"}

with

$$B_{t} = B_{t - 1} - E_{t - 1}^{B}$$ {#eq:DB_2 tag="DB.2"}

where

-   $E^{B}$ are emissions (in million metric tonnes of carbon);

-   $t$ denotes time;

-   $T$ is the global mean temperature (in degree Celsius);

-   $B_{t}$ is the remaining stock of potential emissions (in million
    metric tonnes of carbon, GtC);

-   $B_{\mathrm{\max}}$ is the total stock of potential emissions;
    $B_{\mathrm{\max}} = 1,900\ GtC$;

-   $\beta$ is a parameter; $\beta = 2.6\frac{\text{GtC}}{C}$ (with a
    gamma distribution with shape=4.9 and scale=662.8).

The model is calibrated to the review of (Denman et al. 2007). Emissions
from the terrestrial biosphere before the year 2010 are zero.

4. Atmosphere and climate
=========================

4.1. Concentrations
-------------------

Methane, nitrous oxide and sulphur hexafluoride are taken up in the
atmosphere, and then geometrically depleted:

$$C_{t} = C_{t - 1} + \alpha E_{t} - \beta\left( C_{t - 1} - C_{\text{pre}} \right)$$ {#eq:C_1 tag="C.1"}

where $C$ denotes concentration, $E$ emissions, $t$ year, and
$\text{pre}$ pre-industrial. Table C displays the parameters $\alpha$
and $\beta$ for all gases. Parameters are taken from Forster *et al*.
(2007).

The atmospheric concentration of carbon dioxide follows from a five-box
model:

$$Box_{i,t} = \rho_{i}Box_{i,t - 1} + 0.000471\alpha_{i}E_{t}$$ {#eq:C_2a tag="C.2a"}

with

$$C_{t} = \sum_{i = 1}^{5}{\alpha_{i}\text{Bo}x_{i,t}}$$ {#eq:C_2b tag="C.2b"}

where $\alpha_{i}$ denotes the fraction of emissions $E$ (in million
metric tonnes of carbon) that is allocated to $Box_{i}$ (0.13,
0.20, 0.32, 0.25 and 0.10, respectively) and $\rho$ the decay-rate of
the boxes ($\rho = exp( - \frac{1}{\mathrm{\text{lifetime}}})$, with
life-times infinity, 363, 74, 17 and 2 years, respectively). The model
is due to Maier-Reimer and Hasselmann (1987), its parameters are due to
Hammitt *et al.* (1992). Thus, 13% of total emissions remains forever in
the atmosphere, while 10% is—on average—removed in two years. Carbon
dioxide concentrations are measured in parts per million by volume.

4.2. Radiative forcing
----------------------

Radiative forcing is specified as follows:

$$\begin{aligned}
  RF_{t} = &5.35\ln\frac{\text{CO}2_{t}}{275}\\
  &+ 0.036 \times 1.4\left( \sqrt{\text{CH}4_{t}} - \sqrt{790} \right) + 0.12\left( \sqrt{N2O_{t}} - \sqrt{285} \right) \\
  &-0.47\ln\left( 1 + 2.01 \times 10^{- 5}\text{CH}4_{t}^{0.75}285^{0.75} + 5.31 \times 10^{- 15}\text{CH}4_{t}^{2.52}285^{1.52} \right) \\
  &- 0.47\ln\left( 1 + 2.01 + 10^{- 5}790^{0.75}N2O_{t}^{0.75} + 5.31 \times 10^{- 15}790^{2.52}N2O_{t}^{1.52} \right) \\
  &+ 2 \times 0.47\ln\left( 1 + 2.01 \times 10^{- 5}790^{0.75}285^{0.75} + 5.31 \times 10^{- 15}790^{2.52}285^{1.52} \right) \\
  &+ 0.00052\left( \text{SF}6_{t} - 0.04 \right) + rfSO2_{t}
\end{aligned}$$ {#eq:C_3 tag="C.3"}

Parameters are taken from Ramaswamy *et al.* (2001) and Forster et al.
(2007) for the indirect effect of methane on tropospheric ozone.
Radiative forcing from SO2 at time t ($\text{rfSO}2_{t}$) is exogenous;
the FUND scenario uses the forcing from RCP85 and the SRES scenarios use
the forcing as interpreted by IMAGE 2.2.

4.3. Temperature and sea level rise
-----------------------------------

The global mean temperature $T$ is governed by a geometric build-up to
its equilibrium (determined by radiative forcing $RF$). In the
base case, global mean temperature $T$ rises in equilibrium by 3.0°C for
a doubling of carbon dioxide equivalents, so:

$$T_{t} = \left( 1 - \frac{1}{\varphi} \right)T_{t - 1} + \frac{1}{\varphi}\frac{CS}{5.35\ln 2}RF_{t}$$ {#eq:C_4 tag="C.4"}

where $CS$ is climate sensitivity, set to 3.0 (with a gamma
distribution with shape=6.48 and scale=0.55). $\varphi$ is the e-folding
time and set to

$$\varphi = \max\left( \alpha + \beta^{l}CS + \beta^{q}CS^{2},1 \right)$$ {#eq:C_5 tag="C.5"}

where $\alpha$ is set to -42.7, $\beta^{l}$ is set to 29.1 and
$\beta^{q}$ is set to 0.001, such that the best guess e-folding time for
a climate sensitvitiy of 3.0 is 44 years.

Regional temperature is derived by multiplying the global mean
temperature by a fixed factor (see Table RT) which corresponds to the
spatial climate change pattern averaged over 14 GCMs (Mendelsohn et al.
2000).

Global mean sea level is also geometric, with its equilibrium level
determined by the temperature and a life-time of 500 years:

$$S_{t} = \left( 1 - \frac{1}{\rho} \right)S_{t - 1} + \gamma\frac{1}{\rho}T_{t}$$ {#eq:C_6 tag="C.6"}

where $\rho = 500$ (with a triangular distribution bounded by 250 and
1000) is the e-folding time. $\gamma = 2$ (with a gamma distribution
with shape=6 and scale=0.4) is sea-level sensitivity to temperature.

Temperature and sea level are calibrated to the best guess temperature
and sea level for the IS92a scenario of Kattenberg *et al.* (1996).

5. Impacts
==========

5.1. Agriculture
----------------

The impacts of climate change on agriculture at time $t$ in region $r$
are split into three parts: impacts due to the rate of climate change
$A_{t,r}^{r}$; impacts due to the level of climate change $A_{t,r}^{l}$;
and impacts from carbon dioxide fertilisation $A_{t,r}^{f}$:

$$A_{t,r} = A_{t,r}^{r} + A_{t,r}^{l} + A_{t,r}^{f}$$ {#eq:A_1 tag="A.1"}

The first part (rate) is always negative: As farmers have imperfect
foresight and are locked into production practices, climate change
implies that farmers are maladapted. Faster climate change means greater
damages. The third part (fertilization) is always positive. CO~2~
fertilization means that plants grow faster and use less water. The
second part (level) can be positive or negative. There is an optimal
climate for agriculture. If climate change moves a region closer to
(away from) the optimum, impacts are positive (negative); and impacts
are smaller nearer to the optimum.

For the impact of the rate of climate change (i.e., the annual change of
climate) on agriculture, the assumed model is:

$$A_{t,r}^{r} = \alpha_{r}\left( \frac{\Delta T_{t}}{0.04} \right)^{\beta} + \left( 1 - \frac{1}{\rho} \right)A_{t - 1,r}^{r}$$ {#eq:A_2 tag="A.2"}

where

-   $A^{r}$ denotes damage in agricultural production as a fraction due
    the rate of climate change by time and region;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\Delta T$ denotes the change in the regional mean temperature (in
    degrees Celsius) between time $t$ and $t - 1$;

-   $\alpha$ is a parameter, denoting the regional change in
    agricultural production for an annual warming of 0.04°C (see Table
    A, column 2-3);

-   $\beta$ = 2.0 (1.5-2.5) is a parameter, equal for all regions,
    denoting the non-linearity of the reaction to temperature; $\beta$
    is an expert guess;

-   $\rho$ = 10 (5-15) is a parameter, equal for all regions, denoting
    the speed of adaptation; $\rho$ is an expert guess.

The model for the impact due to the level of climate change since 1990
is:

$$A_{t,r}^{l} = \delta_{r}^{l}T_{t} + \delta_{r}^{q}T_{t}^{2}$$ {#eq:A_3 tag="A.3"}

where

-   $A^{l}$ denotes the damage in agricultural production as a fraction
    due to the level of climate change by time and region;

-   $t$ denotes time;

-   $r$ denotes region;

-   $T$ denotes the change (in degree Celsius) in regional mean
    temperature relative to 1990;

-   $\delta_{r}^{l}$ and $\delta_{r}^{q}$ are parameters (see Table A),
    that follow from the regional change (in per cent) in agricultural
    production for a warming of 2.5°C above today or 3.2°C above
    pre-industrial and the the optimal temperature (in degree Celsius)
    for agriculture in each region.

CO~2~ fertilisation has a positive, but saturating effect on
agriculture, specified by

$$A_{t,r}^{f} = \gamma_{r}\ln\frac{\text{CO}2_{t}}{275}$$ {#eq:A_4 tag="A.4"}

where

-   $A^{f}$ denotes damage in agricultural production as a fraction due
    to the CO2 fertilisation by time and region;

-   $t$ denotes time;

-   $r$ denotes region;

-   $CO2$ denotes the atmospheric concentration of carbon dioxide (in
    parts per million by volume);

-   275 ppm is the pre-industrial concentration;

-   $\gamma$ is a parameter (see Table A, column 8-9).

The parameters in Table A are calibrated, following the procedure
described in Tol (2002a), to the results of Kane *et al*. (1992), Reilly
*et al*. (1994), Morita *et al*. (1994), Fischer *et al*. (1996), and
Tsigas *et al*. (1996). These studies all use a global computable
general equilibrium model, and report results with and without
adaptation, and with and without CO~2~ fertilisation. The regional
results from these studies are assumed to hold for each country in the
respective regions. They are averaged over the studies and the climate
scenarios for each country, and aggregated to the *FUND* regions. The
standard deviations in Table A follow from the spread between studies
and scenarios. Equation (@eq:A_4) follows from the difference in results
with and without CO2 fertilization. Equation (@eq:A_3) follows from the
results with full adaptation. Equation (@eq:A_2) follows from the difference
in results with and without adaptation.

Equations (@eq:A_1)-(@eq:A_4) express the impact of climate change as a percentage
of agricultural production. In order to express this as a percentage of
income, we need to know the share of agricultural production in total
income. This is assumed to fall with per capita income, that is,

$$\frac{\text{GA}P_{t,r}}{Y_{t,r}} = \frac{\text{GA}P_{1990,r}}{Y_{1990,r}}\left( \frac{y_{1990,r}}{y_{t,r}} \right)^{\epsilon}$$ {#eq:A_5 tag="A.5"}

where

-   $\text{GAP}$ denotes gross agricultural product (in 1995 US dollar
    per year) by time and region;

-   $Y$ denotes gross domestic product (in 1995 US dollar per year) by
    time and region;

-   $y$ denotes gross domestic product per capita (in 1995 US dollar per
    person per year) by time and region;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\epsilon$ = 0.31 (0.15-0.45) is a parameter; it is the income
    elasticity of the share of agriculture in the economy; it is taken
    from Tol (2002b), who regressed the regional share in agriculture on
    per capita income, using 1995 data from the World Resources
    Institute (<http://earthtrends.wri.org>).

5.2. Forestry
-------------

The model is:

$$F_{t,r} = \alpha_{r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\left( 0.5\left( \frac{T_{t}}{1.0} \right)^{\beta} + 0.5\gamma\ln\left( \frac{\text{CO}2_{t}}{275} \right) \right)$$ {#eq:F_1 tag="F.1"}

where

-   $F$ denotes the change in forestry consumer and producer surplus (as
    a share of total income);

-   $t$ denotes time;

-   $r$ denotes region;

-   $y$ denotes per capita income (in 1995 US dollar per person per
    year);

-   $T$ denotes the global mean temperature (in degree centigrade);

-   $\alpha$ is a parameter, that measures the impact of climate change
    of a 1ºC global warming on economic welfare; see Table EFW;

-   $\epsilon$ = 0.31 (0.11-0.51) is a parameter, and equals the income
    elasticity for agriculture;

-   $\beta$ = 1 (0.5-1.5) is a parameter; this is an expert guess;

-   $\gamma$ = 0.44 (0.29-0.87) is a parameter; $\gamma$ is such that a
    doubling of the atmospheric concentration of carbon dioxide would
    lead to a change of forest value of 15% (10-30%); this parameter is
    taken from Gitay *et al*., (2001).

The parameter $\alpha$ is estimated as the average of the estimates by
Perez-Garcia *et al*. (1995) and Sohngen *et al*. (2001). Perez-Garcia
*et al*. (1995) present results for four different climate scenarios and
two management scenarios, while Sohngen *et al*. (2001) use two
different climate scenario and two alternative ecological scenarios. The
results are mapped to the FUND regions assuming that the impact is
uniform elative to GDP. The impact is averaged within the study results,
and then the weighted average between the two studies is computed and
shown in Table EFW. The standard deviation follows.

5.3. Water resources
--------------------

The impact of climate change on water resources follows:

$$W_{t,r} = \min\left\{ \alpha_{r}Y_{1990,r}\left( 1 - \tau \right)^{t - 2000}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\beta}\left( \frac{P_{t,r}}{P_{1990,r}} \right)^{\eta}\left( \frac{T_{t}}{1.0} \right)^{\gamma},\frac{Y_{t,r}}{10} \right\}$$ {#eq:W_1 tag="W.1"}

where

-   $W$ denotes the change in water resources (in 1995 US dollar) at
    time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $y$ denotes per capita income (in 1995 US dollar) at time $t$ in
    region $r$;

-   $P$ denotes population at time $t$ in region $r$;

-   $T$ denotes the global mean temperature above pre-industrial (in
    degree Celsius) at time $t$;

-   $\alpha$ is a parameter (in percent of 1990 GDP per degree Celsius)
    that specifies the benchmark impact; see Table EFW;

-   $\beta$ = 0.85 (0.15, &gt;0) is a parameter, that specifies how
    impacts respond to economic growth;

-   $\eta$ = 0.85 (0.15,&gt;0) is a parameter that specifies how impacts
    respond to population growth;

-   $\gamma$ = 1 (0.5,&gt;0) is a parameter, that determines the
    response of impact to warming;

-   $\tau$ = 0.005 (0.005, &gt;0) is a parameter, that measures
    technological progress in water supply and demand.

These parameters are from calibrating *FUND* to the results of Downing
*et al*. (1995, 1996).

5.4. Energy consumption
-----------------------

For space heating, the model is:

$$SH_{t,r} = \frac{\alpha_{r}Y_{1990,r}\frac{\operatorname{atan}T_{t}}{\operatorname{atan}{1.0}}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\left( \frac{P_{t,r}}{P_{1990,r}} \right)}{\prod_{s = 1990}^{t}{\text{AEE}I_{s,r}}}$$ {#eq:E_1 tag="E.1"}

where

-   $\text{SH}$ denotes the decrease in expenditure on space heating (in
    1995 US dollar) at time $t$ in region $r$;

-   $\text{t\ }$denotes time;

-   $r$ denotes region;

-   $Y$ denotes income (in 1995 US dollar) at time $t$ in region $r$;

-   $T$ denotes the change in the global mean temperature relative to
    1990 (in degree Celsius) at time $t$;

-   $y$ denotes per capita income (in 1995 US dollar per person
    per year) at time $t$ in region $r$;

-   $P$ denotes population size at time $t$ in region $r$;

-   $\alpha$ is a parameter (in dollar per degree Celsius), that
    specifies the benchmark impact; see Table EFW, column 6-7

-   $\epsilon$ is a parameter; it is the income elasticity of space
    heating demand; $\epsilon$ = 0.8 (0.1,&gt;0,&lt;1);

-   $\text{AEEI}$ is a parameter (cf. Tables AEEI and Equation @eq:CO2_3);
    it is the Autonomous Energy Efficiency Improvement, measuring
    technological progress in energy provision; the global average value
    is about 1% per year in 1990, converging to 0.2% in 2200; its
    standard deviation is set at a quarter of the mean.

These parameters are from calibrating *FUND* to the results of Downing
*et al*. (1995, 1996). Savings on space heating are assumed to saturate.
The income elasticity of heating demand is taken from Hodgson and Miller
(1995, cited in Downing *et al*., 1996), and estimated for the . Space
heating demand is linear in the number of people for want of scenarios
of number of households and house sizes. Energy efficiency improvements
in space heating are assumed to be equal to the average energy
efficiency improvements in the economy.

For space cooling, the model is:

$$SC_{t,r} = \frac{\alpha_{r}Y_{1990,r}\left( \frac{T_{t}}{1.0} \right)^{\beta}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\left( \frac{P_{t,r}}{P_{1990,r}} \right)}{\prod_{s = 1990}^{t}{\text{AEE}I_{s,r}}}$$ {#eq:E_2 tag="E.2"}

where

-   $\text{SC}$ denotes the increase in expenditure on space cooling
    (1995 US dollar) at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $Y$ denotes income (in 1995 US dollar) at time $t$ in region $r$;

-   $T$ denotes the change in the global mean temperature relative to
    1990 (in degree Celsius) at time $t$;

-   $y$ denotes per capita income (in 1995 US dollar per person
    per year) at time $t$ in region $r$;

-   $P$ denotes population size at time $t$ in region $r$;

-   $\alpha$ is a parameter (see Table EFW, column 8-9);

-   $\beta$ is a parameter; $\beta$ = 1.5 (1.0-2.0);

-   $\epsilon$ is a parameter; it is the income elasticity of space
    heating demand; $\epsilon$ = 0.8 (0.6-1.0);

-   $\text{AEEI}$ is a parameter (cf. Tables AEEI and Equation @eq:CO2_3) ;
    it is the Autonomous Energy Efficiency Improvement, measuring
    technological progress in energy provision; the global average value
    is about 1% per year in 1990, converging to 0.2% in 2200; its
    standard deviation is set at a quarter of the mean.

These parameters are from calibrating *FUND* to the results of Downing
*et al*. (1995, 1996). Space cooling is assumed to be more than linear
in temperature because cooling demand accelerates as it gets warmer. The
income elasticity of cooling demand is taken from Hodgson and Miller
(1995, cited in Downing *et al*., 1996), and estimated for the . Space
cooling demand is linear in the number of people for want of scenarios
of number of households and house sizes. Energy efficiency improvements
in space cooling are assumed to be equal to the average energy
efficiency improvements in the economy.

5.5. Sea level rise
-------------------

Table SLR shows the accumulated loss of drylands and wetlands for a one
metre rise in sea level. The data are taken from Hoozemans et al.
(1993), supplemented by data from Bijlsma et al. (1995), Leatherman and
Nicholls (1995) and Nicholls and Leatherman (1995), following the
procedures of Tol (2002a).

Potential cumulative dryland loss without protection is assumed to be a
function of sea level rise:

$${\overset{\overline{}}{\text{CD}}}_{t,r} = \min\left\lbrack \delta_{r}S_{t}^{\gamma_{r}},\zeta_{r} \right\rbrack$$ {#eq:SLR_1 tag="SLR.1"}

where

-   ${\overset{\overline{}}{\text{CD}}}_{t,r}$ is the potential
    cumulative dryland lost at time $t$ in region $r$ that would occur
    without protection;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\delta_{r}$ is the dryland loss due to one metre sea level rise (in
    square kilometre per metre) in region $r$;

-   $S_{t}$ is sea level rise above pre-industrial levels at time $t$;
    note that is assumed to equal for all regions;

-   $\gamma_{r}$ is a parameter, calibrated to a digital elevation
    model;

-   $\zeta_{r}$ is the maximum dryland loss in region $r$, which is
    equal to the area in the year 2000.

Potential dryland loss in the current year without protection is given
by potential cumulative dryland loss without protection minus actual
cumulative dryland lost in previous years:

$${\overset{\overline{}}{D}}_{t,r} = {\overset{\overline{}}{\text{CD}}}_{t,r} - CD_{t - 1,r}$$ {#eq:SLR_2 tag="SLR.2"}

where

-   ${\overset{\overline{}}{D}}_{t,r}$ is potential dryland loss in year
    $t$ and region $r$ without protection;

-   ${\overset{\overline{}}{\text{CD}}}_{t,r}$ is the potential
    cumulative dryland lost at time $t$ in region $r$ that would occur
    without protection;

-   $CD_{t,r}$ is the actual cumulative dryland lost at time $t$ in
    region $r$.

Actual dryland loss in the current year depends on the level of
protection:

$$D_{t,r} = \left( 1 - P_{t,r} \right){\overset{\overline{}}{D}}_{t,r}$$ {#eq:SLR_3 tag="SLR.3"}

where

-   $D_{t,r}$ is dryland loss in year $t$ and region $r$;

-   $P_{t,r}$ is the fraction of the coastline protected in year $t$ and
    region $r$;

-   ${\overset{\overline{}}{D}}_{t,r}$ is potential dryland loss in year
    $t$ and region $r$ without protection.

Actual cumulative dryland loss is given by:

$$\text{CD}_{t,r} = CD_{t - 1,r} + D_{t,r}$$ {#eq:SLR_4 tag="SLR.4"}

where

-   $CD_{t,r}$ is the actual cumulative dryland lost at time $t$ in
    region $r$;

-   $D_{t,r}$ is dryland loss in year $t$ and region $r$.

The value of dryland is assumed to be linear in income density
(\$/km^2^):

$$VD_{t,r} = \varphi\left( \frac{\frac{Y_{t,r}}{A_{t,r}}}{YA_{0}} \right)^{\epsilon}$$ {#eq:SLR_5 tag="SLR.5"}

where

-   $\text{VD}$ is the unit value of dryland (in million dollar per
    square kilometre) at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $Y$ is the total income (in billion dollar) at time $t$ in region
    $r$;

-   $A$ is the area (in square kilometre) at time $t$ of region $r$;

-   $\varphi$ is a parameter; $\varphi$ = 4 (2,&gt;0) million dollar per
    square kilometre (Darwin *et al*., 1995);

-   $YA_{0}$ =0.635 (million dollar per square kilometre) is a
    normalisation constant, the average incomde density of the OECD in
    1990;

-   $\epsilon$ is a parameter, the income density elasticity of land
    value; $\epsilon$ = 1 (0.25).

Wetland loss is assumed to be a linear function of sea level rise:

$$W_{t,r} = \omega_{r}^{S}\Delta S_{t} + \omega_{r}^{M}P_{t,r}\Delta S_{t}$$ {#eq:SLR_6 tag="SLR.6"}

where

-   $W_{t,r}$ is the wetland lost at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $P_{t,r}$ is fraction of coast protected against sea level rise at
    time $t$ in region $r$;

-   $\Delta S_{t}$ is sea level rise at time $t$; note that is assumed
    to equal for all regions;

-   $\omega^{S}$ is a parameter, the annual unit wetland loss due to sea
    level rise (in square kilometre per metre) in region $r$; note that
    is assumed to be constant over time;

-   $\omega^{M}$ is a parameter, the annual unit wetland loss due to
    coastal squeeze (in square kilometre per metre) in region $r$; note
    that is assumed to be constant over time.

Cumulative wetland loss is given by

$$W_{t,r}^{C} = \min\left( W_{t - 1,r}^{C} + W_{t - 1,r},W_{r}^{M} \right)$$ {#eq:SLR_7 tag="SLR.7"}

where

-   $W^{C}$ is cumulative wetland loss (in square kilometre) at time $t$
    in region $r$

-   $W^{M}$ is a parameter, the total amount of wetland that is exposed
    to sea level rise; this is assumed to be smaller than the total
    amount of wetlands in 1990.

Wetland loss (SLR.6) goes to zero if all wetland threatened by sea-level
rise in a region is lost.

Wetland value is assumed to increase with income and population density,
and fall with wetland size:

$$VW_{t,r} = \alpha\left( \frac{y_{t,r}}{y_{0}} \right)^{\beta}\left( \frac{d_{t,r}}{d_{0}} \right)^{\gamma}\left( \frac{W_{1990,r} - W_{t,r}^{C}}{W_{1990,r}} \right)^{\delta}$$ {#eq:SLR_8 tag="SLR.8"}

where

-   $\text{VW}$ is the wetland value (in dollar per square kilometre) at
    time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $y$ is per capita income (in dollar per person per year) at time $t$
    in region $r$;

-   $d$ is population density (in person per square kilometre) at time
    $t$ in region $r$;

-   $W^{C}$ is cumulative wetland loss (in square kilometre) at time $t$
    in region $r$;

-   $W_{1990}$ is the total amount of wetlands in 1990 in region $r$;

-   $\alpha$ is a parameter, the net present value of the future stream
    of wetland services; note that we thus account present and future
    wetland values in the year that the wetland is lost;
    $\alpha = \alpha^{'}\frac{1 + \rho + \eta g_{t,r}}{\rho + \eta g_{t,r}} = \alpha^{'}\frac{1 + 0.03 + 1 \times 0.02}{0.03 + 1 \times 0.02} = 21\alpha^{'}$

-   $\alpha^{'}$ = 280,000 \$/km^2^, with a standard deviation of
    187,000 \$/km^2^; $\alpha$ is the average of the meta-analysis of
    Brander et al. (2006); the standard deviation is based on the
    coefficient of variation of the intercept in their analysis;

-   $\beta$ is a parameter, the income elasticity of wetland value;
    $\beta$ = 1.16 (0.46,&gt;0); this value is taken from Brander et al.
    (2006);

-   $y_{0}$ is a normalisation constant; $y_{0}$ = 25,000 \$/p/yr
    (Brander, personal communication);

-   $d_{0}$ is a normalisation constant; $d_{0}$ = 27.59;

-   $\gamma$ is a parameter, the population density elasticity of
    wetland value; $\gamma$ = 0.47 (0.12,&gt;0,&lt;1); this value is
    taken from Brander et al. (2006);

-   $\delta$ is a parameter, the size elasticity of wetland value;
    $\delta$ = -0.11 (0.05,&gt;-1,&lt;0); this value is taken from
    Brander et al. (2006);

If dryland gets lost, the people living there are forced to move. The
number of forced migrants follows from the amount of land lost and the
average population density in the region. The value of this is set at 3
(1.5,&gt;0) times the regional per capita income per migrant (Tol,
1995). In the receiving country, costs equal 40% (20%,&gt;0) of per
capita income per migrant (Cline, 1992).

Table SLR displays the annual costs of fully protecting all coasts
against a one metre sea level rise in a hundred years time. If sea level
would rise slower, annual costs are assumed to be proportionally lower;
that is, costs of coastal protection are linear in sea level rise. The
level of protection, that is, the share of the coastline protected, is
based on a cost-benefit analysis:

$$P_{t,r} = \max\left\{ 0,1 - \frac{1}{2}\left( \frac{\mathrm{\text{NPV}}VP_{t,r} + \mathrm{\text{NPV}}VW_{t,r}}{\mathrm{\text{NPV}}VD_{t,r}} \right) \right\}$$ {#eq:SLR_9 tag="SLR.9"}

where

-   $P$ is the fraction of the coastline to be protected;

-   $\mathrm{\text{NPV}}\text{VP}$ is the net present value of the
    protection if the whole coast is protected (defined below);

-   $\mathrm{\text{NPV}}\text{VW}$ is the net present value of the
    wetland lost due to coastal squeeze if the whole coast is protected
    (defined below);

-   $\mathrm{\text{NPV}}\text{VD}$ is the net present value of the land
    lost without any coastal protection (defined below).

Equation (@eq:SLR_9) is due to Fankhauser (1994). See below.

Table SLR reports average costs per year over the next century. $\mathrm{\text{NPV}}\text{VP}$
is calculated assuming annual costs to be constant. This is based on the
following. Firstly, the coastal protection decision makers anticipate a
linear sea level rise. Secondly, coastal protection entails large
infrastructural works which last for decades. Thirdly, the considered
costs are direct investments only, and technologies for coastal
protection are mature. Throughout the analysis, a pure rate of time
preference, $\rho$, of 1% per year is used. The actual discount rate
lies thus 1% above the growth rate of the economy, $g$. The net present
costs of protection $PC$ equal

$$\mathrm{\text{NPV}}VP_{t,r} = \sum_{s = t}^{\infty}{\left( \frac{1}{1 + \rho + \eta g_{t,r}} \right)^{s - t}\pi_{r}\Delta S_{t}} = \frac{1 + \rho + \eta g_{t,r}}{\rho + \eta g_{t,r}}\pi_{r}\Delta S_{t}$$ {#eq:SLR_10 tag="SLR.10"}

where

-   $\mathrm{\text{NPV}}\text{VP}$ is the net present costs of coastal
    protection at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\pi_{r}$ is the annual unit cost of coastal protection (in million
    dollar per vertical metre) in region $r$; note that is assumed to be
    constant over time;

-   $\Delta S_{t}$ is sea level rise at time $t$; note that is assumed
    to equal for all regions;

-   $g$ is the growth rate of per capita income at time $t$ in region
    $r$;

-   $\rho$ is a parameter, the rate of pure time preference; $\rho$ =
    0.03;

-   $\eta$ is a parameter, the consumption elasticity of marginal
    utility; $\eta$ = 1;

$\mathrm{\text{NPV}}\text{VW}$ is the net present value of the wetlands
lost due to full coastal protection. Wetland values are assumed to rise
in line with Equation (@eq:SLR_8). All growth rates and the rate of wetland
loss are as in the current year. The net present costs of wetland loss
$\text{WL}$ follow from

$$\mathrm{\text{NPV}}VW_{t,r} = \sum_{s = t}^{\infty}{W_{t,r}VW_{s,r}\left( \frac{1}{1 + \rho + \eta g_{t,r}} \right)^{s - t}} = W_{t,r}VW_{t,r}\frac{1 + \rho + \eta g_{t,r}}{\rho + \eta g_{t,r} - \beta g_{t,r} - \gamma p_{t,r} - \delta w_{t,r}}$$ {#eq:SLR_11 tag="SLR.11"}

where

-   $\mathrm{\text{NPV}}\text{VW}$ denotes the net present value of
    wetland loss. at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\omega_{r}$ is the annual unit wetland loss due to full coastal
    protection (in square kilometre per metre sea level rise) in region
    $r$; note that is assumed to be constant over time;

-   $\Delta S_{t}$ is sea level rise at time $t$; note that is assumed
    to equal for all regions;

-   $g$ is the growth rate of per capita income at time $t$ in region
    $r$;

-   $p$ is the population growth rate at time $t$ in region $r$;

-   $w$ is the growth rate of wetland at time $t$ in region $r$; note
    that wetlands shrink, so that $w < 0$;

-   $\rho$ is a parameter, the rate of pure time preference; $\rho$ =
    0.03;

-   $\eta$ is a parameter, the consumption elasticity of marginal
    utility; $\eta$ = 1;

-   $\beta$ is a parameter, the income elasticity of wetland value;
    $\beta$ = 1.16 (0.46,&gt;0); this value is taken from Brander et al.
    (2006);

-   $\gamma$ is a parameter, the population density elasticity of
    wetland value; $\gamma$ = 0.47 (0.12,&gt;0,&lt;1); this value is
    taken from Brander et al. (2006);

-   $\delta$ is a parameter, the size elasticity of wetland value;
    $\delta$ = -0.11 (0.05,&gt;-1,&lt;0); this value is taken from
    Brander et al. (2006);

$\mathrm{\text{NPV}}\text{VD}$ denotes the net present value of the
dryland lost if no protection takes place. Land values are assumed to
rise at the rate of income growth. All growth rates and the rate of
wetland loss are as in the current year. The net present costs of
dryland loss are

$$\mathrm{\text{NPV}}VD_{t,r} = \sum_{s = t}^{\infty}{{\overset{\overline{}}{D}}_{t,r}VD_{t,r}\left( \frac{1 + \epsilon d_{t,r}}{1 + \rho + \eta g_{t,r}} \right)^{s - t}} = {\overset{\overline{}}{D}}_{t,r}VD_{t,r}\frac{1 + \rho + \eta g_{t,r}}{\rho + \eta g_{t,r} - \epsilon d_{t,r}}$$ {#eq:SLR_12 tag="SLR.12"}

where

-   $\mathrm{\text{NPV}}\text{VD}$ is the net present value of dryland
    loss at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $\overset{\overline{}}{D}$ is the current dryland loss without
    protection at time $t$ in region $r$;

-   $\text{VD}$ is the current dryland value;

-   $g$ is the growth rate of per capita income at time $t$ in region
    $r$;

-   $\rho$ is a parameter, the rate of pure time preference;
    $\rho = 0.03$;

-   $\eta$ is a parameter, the consumption elasticity of marginal
    utility; $\eta = 1$;

-   $\epsilon$ is a parameter, the income elasticity of dryland value;
    $\epsilon = 1.0$, with a standard deviation of 0.2;

-   $d$ is the current income density growth rate at time $t$ in region
    $r$.

Protection levels are bounded between 0 and 1.

5.6. Ecosystems
---------------

Tol (2002a) assesses the impact of climate change on ecosystems,
biodiversity, species, landscape *etcetera* based on the "warm-glow"
effect. Essentially, the value, which people are assumed to place on
such impacts, are independent of any real change in ecosystems, of the
location and time of the presumed change, *etcetera* – although the
probability of detection of impacts by the “general public” is
increasing in the rate of warming. This value is specified as

$$E_{t,r} = \alpha P_{t,r}\frac{\frac{y_{t,r}}{y_{r}^{b}}}{1 + \frac{y_{t,r}}{y_{r}^{b}}}\frac{\frac{\Delta T_{t}}{\tau}}{1 + \frac{\Delta T_{t}}{\tau}}\left( 1 - \sigma + \sigma\frac{B_{0}}{B_{t}} \right)$$ {#eq:E_1 tag="E.1"}

where

-   $E$ denotes the value of the loss of ecosystems (in 1995 US dollar)
    at time $t$ in region $r$;

-   $t$ denotes time;

-   $r$ denotes region;

-   $y$ denotes per capita income (in 1995 dollar per person per year)
    at time $t$ in region $r$;

-   $P$ denotes population size (in millions) at time $t$ in region $r$;

-   $\Delta T$ denotes the change in temperature (in degree Celsius);

-   $B$ is the number of species, which makes that the value increases
    as the number of species falls – using Weitzman’s (1998) ranking
    criterion and Weitzman’s (1992, 1993) biodiversity index, the
    scarcity value of biodiversity is inversely proportional to the
    number of species;

-   $\alpha$=50 (0-100, &gt;0) is a parameter such that the value equals
    \$50 per person if per capita income equals the OECD average in 1990
    (Pearce and Moran, 1994);

-   $y^{b}$ = is a parameter; $y^{b}$ = \$30,000, with a standard
    deviation of \$10,000; it is normally distributed, but knotted
    at zero.

-   $\tau$=0.025ºC is a parameter;

-   $\sigma$=0.05 (triangular distribution,&gt;0,&lt;1) is a parameter,
    based on an expert guess; and

-   $B_{0}$ =14,000,000 is a parameter.

The number of species follows

$$B_{t} = \max\left\{ \frac{B_{0}}{100},B_{t - 1}\left( 1 - \rho - \gamma\frac{\Delta T^{2}}{\tau^{2}} \right) \right\}$$ {#eq:E_2 tag="E.2"}

where

-   $\rho$ = 0.003 (0.001-0.005, &gt;0.0) is a parameter;

-   $\gamma$ = 0.001 (0.0-0.002, &gt;0.0) is a parameter; and

These parameters are expert guesses. The number of species is assumed to
be constant until the year 2000 at 14,000,000 species.

5.7. Human health: Diarrhoea
----------------------------

The number of additional diarrhoea deaths $D_{t,r}^{d}$ in region $r$
and time $t$ is given by

$$D_{t,r}^{d} = \mu_{r}^{d}P_{t,r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\left( \frac{T_{t,r}}{T_{\mathrm{pre - industrial},r}} \right)^{\eta}$$ {#eq:HD_1 tag="HD.1"}

where

-   $P_{t,r}$ denotes population,

-   $r$ indexes region

-   $t$ indexes time,

-   $y_{t,r}$ is the per capita income in region $r$ and year $t$ in
    1995 US dollars,

-   $T_{t,r}$ is regional temperature in year $t$, in degrees Celcius
    (C);

-   $\mu_{r}^{d}$ is the rate of mortality from diarrhoea in 2000 in
    region $r$, taken from the WHO Global Burden of Disease (see Table
    HD, column 3);

-   $\epsilon$ = -1.58 (0.23)is the income elasticity of diarrhoea
    mortality

-   $\eta$ = 1.14 (0.51) is a parameter, the degree of non-linearity of
    the response of diarrhoea mortality to regional warming.

Equation (@eq:HD_1), specifically parameters $\epsilon$ and $\eta$, was
estimated based on the WHO Global Burden of Diseases data
(http://www.who.int/health\_topics/global\_burden\_of\_disease/en/).
Diarrhoea morbidity has the same equation as mortality, but with
$\epsilon$=-0.42 (0.12) and $\eta$=0.70 (0.26); base morbidity is given
in Table HD, column 4. Table HD gives impact estimates, ignoring
economic and population growth.

See section 5.12. for a description of the valuation of mortality and
morbidity.

5.8. Human health: Vector-borne diseases
----------------------------------------

The number of additional deaths from vector-borne diseases,
$D_{t,r}^{v}$ is given by:

$$D_{t,r}^{v} = D_{1990,r}^{v}\alpha_{r}^{v}T_{t}^{\beta}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\gamma}$$ {#eq:HV tag="HV"}

where

-   $D_{t,r}^{v}$ denotes climate-change-induced mortality due to
    disease $v$ in region $r$ at time $t$;

-   $D_{1990,r}^{v}$ denotes mortality from vector-borne diseases in
    region $r$ in 1990 (see Table HV, column “base”);

-   $t$ denotes time;

-   $r$ denotes region;

-   $v$ denotes vector-borne disease (malaria, schistosomiasis, dengue
    fever);

-   $\alpha$ is a parameter, indicating the benchmark impact of climate
    change on vector-borne diseases (see Table HV, column “impact”); the
    best guess is the average of Martin and Lefebvre (1995), Martens
    *et al.* (1995, 1997) and Morita *et al.* (1995), while the standard
    deviation is the spread between models and the scenarios.

-   $y_{t,r}$ denotes per capita income;

-   $T_{t}$ denotes the mean temperature in year $t$, in degrees Celcius
    (C);

-   $\beta$ = 1.0 (0.5) is a parameter, the degree of non-linearity of
    mortality in warming; the parameter is calibrated to the results of
    Martens *et al*. (1997);

-   $\gamma$ = -2.65 (0.69) is the income elasticity of vector-borne
    mortality, taken from Link and Tol (2004), who regress malaria
    mortality on income for the 14 WHO regions..

See section 5.12. for a description of the valuation of mortality and
morbidity. Morbidity is proportional to mortality, using the factor
specified in Table HM.

5.9. Human health: Cardiovascular and respiratory mortality
-----------------------------------------------------------

Cardiovascular and respiratory disorders are worsened by both extreme
cold and extreme hot weather. Martens (1998) assesses the increase in
mortality for 17 countries. Tol (2002a) extrapolates these findings to
all other countries, based on formulae of the shape:

$$D^{c} = \alpha^{c} + \beta^{c}T_{B}$$ {#eq:HC_1 tag="HC.1"}

where

-   $D^{c}$ denotes the change in mortality (in deaths per
    100,000 people) due to a one degree global warming;

-   $c$ indexes the disease (heat-related cardiovascular under 65,
    heat-related cardiovascular over 65, cold-related cardiovascular
    under 65, cold-related cardiovascular over 65, respiratory);

-   $T_{B}$ is the current temperature of the hottest or coldest month
    in the country (in degree Celsius);

-   $\alpha$ and $\beta$ are parameters, specified in Table HC.1.

Equation (@eq:HC_1) is specified for populations above and below 65 years of
age for cardiovascular disorders. Cardiovascular mortality is affected
by both heat and cold. In the case of heat, $T_{B}$ denotes the average
temperature of the warmest month. In the case of cold, $T_{B}$ denotes
the average temperature of the coldest month. Respiratory mortality is
not age-specific.

Equation (@eq:HC_1) is readily extrapolated. With warming, the baseline
temperature $T_{B}$ changes. If this change is proportional to the
change in the global mean temperature, the equation becomes quadratic.
Summing country-specific quadratic functions results in quadratic
functions for the regions:

$$D_{t,r}^{c} = \alpha_{r}^{c}T_{t} + \beta_{r}^{c}T_{t}^{2}$$ {#eq:HC_2 tag="HC.2"}

where

-   $D_{t,r}^{c}$ denotes climate-change-induced mortality (in deaths
    per 100,000 people) due to disease $c$ in region $r$ at time $t$;

-   $c$ indexes the disease (heat-related cardiovascular under 65,
    heat-related cardiovascular over 65, cold-related cardiovascular
    under 65, cold-related cardiovascular over 65, respiratory);

-   $r$ indexes region;

-   $t$ indexes time;

-   $T_{t}$ denotes the mean temperature in year $t$, in degrees Celcius
    (C);

-   $\alpha$ and $\beta$ are parameters, specified in Tables HC.2-4 (in
    probabilistic mode all probablitiy distributions are constrained so
    that only values with the same sign as the mean can be sampled).

One problem with (@eq:HC_2) is that it is a non-linear extrapolation based
on a data-set that is limited to 17 countries and, more importantly, a
single climate change scenario. A global warming of 1°C leads to changes
in cardiovascular and respiratory mortality in the order of magnitude of
1% of baseline mortality due to such disorders. Per cause, the total
change in mortality is restricted to a maximum of 5% of baseline
mortality, an expert guess. This restriction is binding. Baseline
cardiovascular and respiratory mortality derives from the share of the
population above 65 in the total population.

If the fraction of people over 65 increases by 1%, cardiovascular
mortality increases by 0.0259% (0.0096%). For respiratory mortality, the
change is 0.0016% (0.0005%). These parameters are estimated from the
variation in population above 65 and cardiovascular and respiratory
mortality over the nine regions in 1990, using data from
http://www.who.int/health\_topics/global\_burden\_of\_disease/en/.

Mortality as in equations (@eq:HC_1) and (@eq:HC_2) is expressed as a fraction
of population size. Cardiovascular mortality, however, is separately
specified for younger and older people. In 1990, the per capita income
elasticity of the share of the population over 65 is 0.25 (0.08). This
is estimated using data from http://earthtrends.wri.org

Heat-related mortality is assumed to be limited to urban populations.
Urbanisation is a function of per capita income and population density:

$$U_{t,r} = \frac{\alpha\sqrt{y_{t,r}} + \beta\sqrt{PD_{t,r}}}{1 + \alpha\sqrt{y_{t,r}} + \beta\sqrt{PD_{t,r}}}$$ {#eq:HC_3 tag="HC.3"}

where

-   $U$ is the fraction of people living in cities;

-   $y$ is per capita income (in 1995 \$ per person per year);

-   $\text{PD}$ is population density (in people per square kilometre);

-   $t$ is time;

-   $r$ is region;

-   $\alpha$ and $\beta$ are parameters, estimated from a cross-section
    of countries for the year 1995, using data from
    http://earthtrends.wri.org; $\alpha$=0.031 (0.002) and
    $\beta$=-0.011 (0.005); R^2^=0.66.

See section 5.12. for a description of the valuation of mortality and
morbidity. Morbidity is proportional to mortality, using the factor
specified in Table HM.

5.10. Extreme weather: Tropical storms
--------------------------------------

The economic damage $TD$ due to an increase in the intensity of tropical
storms (hurricanes, typhoons) follows

$$TD_{t,r} = \alpha_{r}Y_{t,r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\left\lbrack \left( 1 + \delta T_{t,r} \right)^{\gamma} - 1 \right\rbrack$$ {#eq:TS_1 tag="TS.1"}

where

-   $t$ denotes time;

-   $r$ denotes region

-   $\text{TD}$ is the damage due to tropical storms (1995 \$ per year)
    in region $r$ at time $t$;

-   $Y$ is the gross domestic product (in 1995 \$ per year) in region
    $r$ at time $t$;

-   $\alpha$ is the current damage as fraction of GDP, specified in
    Table TS; the data are from the CRED EM-DAT database;
    http://www.emdat.be/;

-   $y$ is per capita income (in 1995 \$ per person per year) in region
    $r$ at time $t$;

-   $\epsilon$ is the income elasticity of storm damage; $\epsilon$ =
    -0.514 (0.027;&gt;-1,&lt;0) after Toya and Skidmore (2007);

-   $\delta$ is a parameter, indicating how much wind speed increases
    per degree warming; $\delta$=0.04/ºC (0.005) after WMO (2006);

-   $T$ is the temperature increase since pre-industrial times (in
    degree Celsius) in region $r$ at time $t$;

-   $\gamma$ is a parameter; $\gamma$=3 because the power of the wind in
    the cube of its speed.

The mortality $\text{TM}$ due to an increase in the intensity of
tropical storms (hurricanes, typhoons) follows

$$TM_{t,r} = \beta_{r}P_{t,r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\eta}\left\lbrack \left( 1 + \delta T_{t,r} \right)^{\gamma} - 1 \right\rbrack$$ {#eq:TS_2 tag="TS.2"}

where

-   $t$ denotes time;

-   $r$ denotes region

-   $\text{TM}$ is the mortality due to tropical storms (in people
    per year) in region $r$ at time $t$;

-   $P$ is the population (in people) in region $r$ at time $t$;

-   $\beta$ is the current mortality (as a fraction of population),
    specified in Table TS; the data are from the CRED EM-DAT database;
    http://www.emdat.be/;

-   $y$ is per capita income (in 1995 \$ per person per year) in region
    $r$ at time $t$;

-   $\eta$ is the income elasticity of storm damage; $\eta$ =
    -0.501 (0.051;&lt;0) after Toya and Skidmore (2007);

-   $\delta$ is parameter, indicating how much wind speed increases per
    degree warming; $\delta$=0.04/ºC (0.005) after WMO (2006);

-   $T$ is the temperature increase since pre-industrial times (in
    degree Celsius) in region $r$ at time $t$;

-   $\gamma$ is a parameter; $\gamma$=3 because the power of the wind in
    the cube of its speed.

See section 5.12. for a description of the valuation of mortality and
morbidity.

5.11. Extreme weather: Extratropical storms
-------------------------------------------

The economic damage due to an increase in the intensity of extratropical
storms follows the equation below:

$$\text{ET}D_{t,r} = \alpha_{r}Y_{t,r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\epsilon}\delta_{r}\left\lbrack \left( \frac{C_{CO2,t}}{C_{CO2,pre}} \right)^{\gamma} - 1 \right\rbrack$$ {#eq:ETS_1 tag="ETS.1"}

where

-   $\text{ET}D_{t,r}$ is the damage from extratropical cyclones at time
    $t$ in region $r$;

-   $Y_{t,r}$ is GDP in region $r$ and time $t$;

-   $\alpha_{r}$ is benchmark damage from extratropical cyclones for
    region $r$;

-   $y$ is per capita income at time $t$ in region $r$;

-   $\epsilon$=-0.514(0.027,&gt;-1,&lt;0) is the income elasticity of
    extratropical storm damages (Toya and Skidmore 2007);

-   $\delta_{r}$ is the storm sensitivity to atmospheric CO2
    concentrations for region $r$;

-   $C_{CO2,t}$ is atmospheric CO~2~ concentrations;

-   $C_{CO2,pre}$ is the CO~2~ concentrations in the pre-industrial era;

-   $\gamma$=1 is a parameter.

$$\text{ET}M_{t,r} = \beta_{r}P_{t,r}\left( \frac{y_{t,r}}{y_{1990,r}} \right)^{\varphi}\delta_{r}\left\lbrack \left( \frac{C_{CO2,t}}{C_{CO2,pre}} \right)^{\gamma} - 1 \right\rbrack$$ {#eq:EST_2 tag="EST.2"}

where

-   $\text{ET}M_{t,r}$ is the mortality from extratropical cyclones at
    time $t$ in region $r$;

-   $P_{t,r}$ is population in region $r$ and time $t$;

-   $\beta_{r}$ is benchmark mortality from extratropical cyclones for
    region $r$;

-   $y$ is per capita income at time $t$ in region $r$;

-   $\varphi$=-0.501(0.051,&gt;-1,&lt;0) is the income elasticity of
    extratropical storm mortality (Toya and Skidmore 2007);

-   $\delta_{r}$ is the storm sensitivity to atmospheric CO2
    concentrations for region $r$;

-   $C_{CO2,t}$ is atmospheric CO~2~ concentrations;

-   $C_{CO2,pre}$ is the CO~2~ concentrations in the pre-industrial era;

-   $\gamma$=1 is a parameter.

See section 5.12. for a description of the valuation of mortality and
morbidity.

5.12. Mortality and Morbidity
-----------------------------

The value of a statistical life is given by

$$\text{VS}L_{t,r} = \alpha\left( \frac{y_{t,r}}{y_{0}} \right)^{\epsilon}$$ {#eq:MM_1 tag="MM.1"}

where

-   $\text{VSL}$ is the value of a statistical life at time $t$ in
    region $r$;

-   $\alpha$=4992523 (2496261,&gt;0) is a parameter;

-   $y$ is per capita income at time $t$ in region $r$;

-   $y_{0}$ =24963 is a normalisation constant;

-   $\epsilon$=1 (0.2,&gt;0) is the income elasticity of the value of a
    statistical life;

This calibration results in a best guess value of a statistical life
that is 200 times per capita income (Cline, 1992).

The value of a year of morbidity is given by

$$VM_{t,r} = \beta\left( \frac{y_{t,r}}{y_{0}} \right)^{\eta}$$ {#eq:MM_2 tag="MM.2"}

where

-   $\text{VM}$ is the value of a statistical life at time $t$ in region
    $r$;

-   $\beta$= 19970 (29955,&gt;0) is a parameter;

-   $y$ is per capita income at time $t$ in region $r$;

-   $y_{0}$=24963 is a normalisation constant;

-   $\eta$=1 (0.2,&gt;0) is the income elasticity of the value of a year
    of morbidity;

This calibration results in a best guess value of a year of morbidity
that is 0.8 times per capita income (Navrud, 2001).

Acknowledgements
================

We thank Adriana Ciccone for helpful comments on this documentation.

References
==========

Nakicenovic, N. and R.J. Swart (eds.) (2001), *IPCC Special Report on*
Press, .

Bijlsma, L., C.N.Ehler, R.J.T.Klein, S.M.Kulshrestha, R.F.McLean,
N.Mimura, R.J.Nicholls, L.A.Nurse, H.Perez Nieto, E.Z.Stakhiv,
R.K.Turner, and R.A.Warrick (1996), 'Coastal Zones and Small Islands',
in *Climate Change 1995: Impacts, Adaptations and Mitigation of Climate
Change: Scientific-Technical Analyses -- Contribution of Working Group
II to the Second Assessment Report of the Intergovernmental Panel on
Climate Change*, 1 edn, R.T. Watson, M.C. Zinyowera, and R.H. Moss
(eds.), Cambridge University Press, Cambridge, pp. 289-324.

Cline, W.R. (1992), *The Economics of Global Warming* Institute for
International Economics,

Darwin, R.F., M.Tsigas, J.Lewandrowski, and A.Raneses (1996), 'Land use
and cover in ecological economics', *Ecological Economics*, **17**,
157-181.

Darwin, R.F., M.Tsigas, J.Lewandrowski, and A.Raneses (1995), *World
Agriculture and Climate Change - Economic Adaptations*, U.S. Department
of Agriculture, Washington, D.C., **703**.

Downing, T.E., N.Eyre, R.Greener, and D.Blackwell (1996), *Full Fuel
Cycle Study: Evaluation of the Global Warming Externality for Fossil
Fuel Cycles with and without CO2 Abatament and for Two Reference
Scenarios*, Environmental Change Unit, University of Oxford, Oxford.

Downing, T.E., R.A.Greener, and N.Eyre (1995), *The Economic Impacts of
Climate Change: Assessment of Fossil Fuel Cycles for the ExternE
Project*, and Lonsdale, Environmental Change Unit and Eyre Energy
Environment.

Fankhauser, S. (1994), 'Protection vs. Retreat -- The Economic Costs of
Sea Level Rise', *Environment and Planning A*, **27**, 299-319.

Fischer, G., K.Frohberg, M.L.Parry, and C.Rosenzweig (1993), 'Climate
Change and World Food Supply, Demand and Trade', in *Costs, Impacts, and
Benefits of CO~2~ Mitigation*, Y. Kaya et al. (eds.), pp. 133-152.

Fischer, G., K.Frohberg, M.L.Parry, and C.Rosenzweig (1996), 'Impacts of
Potential Climate Change on Global and Regional Food Production and
Vulnerability', in *Climate Change and World Food Security*, T.E.
Downing (ed.), Springer-Verlag, Berlin, pp. 115-159.

Forster, P., V. Ramaswamy, P. Artaxo, T. Berntsen, R. Betts, D. W.
Fahey, J. Haywood, J. Lean, D. C. Lowe, G. Myhre, J. Nganga, R. Prinn,
G. Raga, M. Schulz and R. V. Dorland (2007). Changes in Atmospheric
Constituents and in Radiative Forcing. *Climate Change 2007: The
Physical Science Basis. Contribution of Working Group I to the Fourth
Assessment Report of the Intergovernmental Panel on Climate Change*. S.
Solomon, D. Qin, M. Manning et al. Cambridge, United Kingdom and New
York, NY, USA, Cambridge University Press.

Gitay, H., S.Brown, W.Easterling, B.P.Jallow, J.M.Antle, M.Apps,
R.Beamish, T.Chapin, W.Cramer, J.Frangi, J.Laine, E.Lin, J.J.Magnuson,
I.Noble, J.Price, T.D.Prowse, T.L.Root, E.-D.Schulze, O.Sitotenko,
B.L.Sohngen, and J.-F.Soussana (2001), 'Ecosystems and their Goods and
Services', in *Climate Change 2001: Impacts, Adaptation and
Vulnerability -- Contribution of Working Group II to the Third
Assessment Report of the Intergovernmental Panel on Climate Change*,
J.J. McCarthy et al. (eds.), Cambridge University Press, Cambridge, pp.
235-342.

Goulder, L.H. and S.H.Schneider (1999), 'Induced technological change
and the attractiveness of CO~2~ abatement policies', *Resource and
Energy Economics*, **21**, 211-253.

Goulder, L.H. and K.Mathai (2000), 'Optimal CO~2~ Abatement in the
Presence of Induced Technological Change', *Journal of Environmental
Economics and Management*, **39**, 1-38.

Hammitt, J.K., R.J.Lempert, and M.E.Schlesinger (1992), 'A
Sequential-Decision Strategy for Abating Climate Change', *Nature*,
**357**, 315-318.

Hodgson, D. and K. Miller (1995), ‘Modelling UK Energy Demand’ in T.
Barker, P. Ekins and N. Johnstone (eds.), *Global Warming and Energy
Demand*, Routledge, London.

Hoozemans, F.M.J., M.Marchand, and H.A.Pennekamp (1993), *A Global
Vulnerability Analysis: Vulnerability Assessment for Population, Coastal
Wetlands and Rice Production and a Global Scale (second, revised
edition)*, Delft Hydraulics, .

Hourcade, J.-C., K.Halsneas, M.Jaccard, W.D.Montgomery, R.G.Richels,
J.Robinson, P.R.Shukla, and P.Sturm (1996), 'A Review of Mitigation Cost
Studies', in *Climate Change 1995: Economic and Social Dimensions --
Contribution of Working Group III to the Second Assessment Report of the
Intergovernmental Panel on Climate Change*, J.P. Bruce, H. Lee, and E.F.
Haites (eds.), Cambridge University Press, Cambridge, pp. 297-366.

Hourcade, J.-C., P.R.Shukla, L.Cifuentes, D.Davis, J.A.Edmonds,
B.S.Fisher, E.Fortin, A.Golub, O.Hohmeyer, A.Krupnick, S.Kverndokk,
R.Loulou, R.G.Richels, H.Segenovic, and K.Yamaji (2001), 'Global,
Regional and National Costs and Ancillary Benefits of Mitigation', in
*Climate Change 2001: Mitigation -- Contribution of Working Group III to
the Third Assessment Report of the Intergovernmental Panel on Climate
Change*, O.R. Davidson and B. Metz (eds.), Cambridge University Press,
Cambridge, pp. 499-559.

IMAGE Team (2001), *The IMAGE 2.2 Implementation of the SRES Scenarios:
A Comprehensive Analysis of Emissions, Climate Change, and Impacts in
the 21st Century*, National Institute for Public Health and the
Environment, Bilthoven, **481508018**.

Kane, S., J.M.Reilly, and J.Tobey (1992), 'An Empirical Study of the
Economic Effects of Climate Change on World Agriculture', *Climatic
Change*, **21**, 17-35.

Kattenberg, A., F.Giorgi, H.Grassl, G.A.Meehl, J.F.B.Mitchell,
R.J.Stouffer, T.Tokioka, A.J.Weaver, and T.M.L.Wigley (1996), 'Climate
Models - Projections of Future Climate', in *Climate Change 1995: The
Science of Climate Change -- Contribution of Working Group I to the
Second Assessment Report of the Intergovernmental Panel on Climate
Change*, 1 edn, J.T. Houghton et al. (eds.), Cambridge University Press,
Cambridge, pp. 285-357.

Leatherman, S.P. and R.J.Nicholls (1995), 'Accelerated Sea-Level Rise
and Developing Countries: An Overview', *Journal of Coastal Research*,
**14**, 1-14.

Leggett, J., W.J.Pepper, and R.J.Swart (1992), 'Emissions Scenarios for
the IPCC: An Update', in *Climate Change 1992 - The Supplementary Report
to the IPCC Scientific Assessment*, 1 edn, vol. 1 J.T. Houghton, B.A.
Callander, and S.K. Varney (eds.), Cambridge University Press,
Cambridge, pp. 71-95.

Link, P.M. and R.S.J. Tol (2004), ‘Possible Economic Impacts of a
Shutdown of the Thermohaline Circulation: An Application of *FUND*’,
*Portuguese Economic Journal*, **3**, 99-114.

Maier-Reimer, E. and K.Hasselmann (1987), 'Transport and Storage of
Carbon Dioxide in the Ocean: An Inorganic Ocean Circulation Carbon Cycle
Model', *Climate Dynamics*, **2**, 63-90.

Martens, W.J.M. (1998), 'Climate Change, Thermal Stress and Mortality
Changes', *Social Science and Medicine*, **46**, (3), 331-344.

Martens, W.J.M., T.H. Jetten, J. Rotmans and L.W. Niessen (1995).
Climate Change and Vector-Borne Diseases -- A Global Modelling
Perspective. *Global Environmental Change* **5** (3):195-209.

Martens, W.J.M., T.H. Jetten and D.A. Focks (1997). Sensitivity of
Malaria, Schistosomiasis and Dengue to Global Warming. *Climatic Change*
**35** 145-156.

Martin, P.H. and M.G. Lefebvre (1995). Malaria and Climate: Sensitivity
of Malaria Potential Transmission to Climate. *Ambio* **24**
(4):200-207.

Mendelsohn, R.O., Schlesinger M.E., Williams L.J. (2000) Comparing
impacts across climate models. Integr Assess 1:37–48.
doi:10.1023/A:1019111327619

Morita, T., M.Kainuma, H.Harasawa, K.Kai, L.Dong-Kun, and Y.Matsuoka
(1994), *Asian-Pacific Integrated Model for Evaluating Policy Options to
Reduce Greenhouse Gas Emissions and Global Warming Impacts*, National
Institute for Environmental Studies, Tsukuba.

Navrud, S. (2001), 'Valuing Health Impacts from Air Pollution in ',
*Environmental and Resource Economics*, **20**, (4), 305-329.

Nicholls, R.J. and S.P.Leatherman (1995), 'The Implications of
Accelerated Sea-Level Rise for Developing Countries: A Discussion',
*Journal of Coastal Research*, **14**, 303-323.

Pearce, D.W. and D.Moran (1994), *The Economic Value of Biodiversity*
EarthScan, .

Perez-Garcia, J., Joyce, L. A., Binkley, C. S., & McGuire, A. D.
"Economic Impacts of Climatic Change on the Global Sector: An Integrated
Ecological/Economic Assessment", Bergendal.

Ramaswamy, V., O.Boucher, J.Haigh, D.Hauglustaine, J.Haywood, G.Myhre,
T.Nakajima, G.Y.Shi, and S.Solomon (2001), 'Radiative Forcing of Climate
Change', in *Climate Change 2001: The Scientific Basis -- Contribution
of Working Group I to the Third Assessment Report of the
Intergovernmental Panel on Climate Change*, J.T. Houghton and Y. Ding
(eds.), Cambridge University Press, Cambridge, pp. 349-416.

Reilly, J.M., N.Hohmann, and S.Kane (1994), 'Climate Change and
Agricultural Trade: Who Benefits, Who Loses?', *Global Environmental
Change*, **4**, (1), 24-36.

Sohngen, B.L., R.O.Mendelsohn, and R.A.Sedjo (2001), 'A Global Model of
Climate Change Impacts on Timber Markets', *Journal of Agricultural and
Resource Economics*, **26**, (2), 326-343.

Tol, R.S.J. (2002), 'Estimates of the Damage Costs of Climate Change -
Part 1: Benchmark Estimates', *Environmental and Resource Economics*,
**21**, 47-73.

Tol, R.S.J. (2002), 'Estimates of the Damage Costs of Climate Change -
Part II: Dynamic Estimates', *Environmental and Resource Economics*,
**21**, 135-160.

Tol, R.S.J. (1995), 'The Damage Costs of Climate Change Toward More
Comprehensive Calculations', *Environmental and Resource Economics*,
**5**, 353-374.

Toya, H. and M. Skidmore (2007), ‘Economic Development and the Impact of
Natural Disasters’, *Economics Letters*, **94**, 20-25.

, , G.B.Frisvold, and B.Kuhn (1996), 'Global Climate Change in
Agriculture', in *Global Trade Analysis: Modelling and Applications*,
T.W. Hertel (ed.), Press, .

USEPA (2003), *International Analysis of Methane and Nitrous Oxide
Abatement Opportunities: Report to Energy Modeling Forum, Working Group
21*, Environmental , D.C..

Weitzman, M.L. (1992), 'On Diversity', *Quarterly Journal of Economics*,
364-405.

Weitzman, M.L. (1998), 'The Noah's Problem', *Econometrica*, **66**,
(6), 1279-1298.

Weitzman, M.L. (1993), 'What to preserve? An application of diversity
theory to crane conservation', *Quarterly Journal of Economics*,
157-183.

Weyant, J.P. (2004), 'Introduction and overview', *Energy Economics*,
**26**, 501-515.

Weyant, J.P., F.C.de la Chesnaye, and G.J.Blanford (2006), 'Overview of
EMF-21: Multigas Mitigation and Climate Policy', *Energy Journal*
(Multi-Greenhouse Gas Mitigation and Climate Policy Special Issue),
1-32.

WMO (2006), *Summary Statement on Tropical Cyclones and Climate Change*,
World Meteorological Organization.
http://www.wmo.ch/pages/prog/arep/tmrp/documents/iwtc\_summary.pdf
