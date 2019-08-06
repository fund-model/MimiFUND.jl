using Documenter, DocumenterLaTeX

makedocs(
	sitename = "MimiFUND.jl",
	pages = [
		"Introduction" => "intro.md",
		"Science" => "science.md",
		"Tables" => "tables.md"]
)

deploydocs(
    repo = "github.com/fund-model/MimiFUND.jl.git"
)
