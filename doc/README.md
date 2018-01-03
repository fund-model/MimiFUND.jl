# FUND documentation

The FUND documentation is written in markdown.

## Editing

To make changes to the documentation, edit the file  ``science.md``.

## Building the pdf version of the documentation

You need to install

- [pandoc](http://pandoc.org/)

To create the pdf, run ``build.ps`` on Windows.

## Mkdocs version

The documentation also gets build and deployed to http://fundjl.readthedocs.org/en/latest/ with every commit. That feature is a bit experimental at the moment, in particular we need to figure out how we can get equation numbering to work.

To view the mkdoc version locally, you need to install

- [MkDocs](http://www.mkdocs.org/)
- [python-markdown-math](https://github.com/mitya57/python-markdown-math)
