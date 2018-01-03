pandoc science-title.md science.md -o fund-scientific.pdf --filter pandoc-eqnos
pandoc tables-title.md tables.md -o fund-tables.pdf --filter pandoc-eqnos --filter pandoc-tablenos -V geometry:margin=0.25in
