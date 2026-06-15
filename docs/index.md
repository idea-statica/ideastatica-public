# IDEA StatiCa Developer Documentation

Automate structural design and code-checking, integrate your FEA or CAD application, and build your own engineering workflows on top of IDEA StatiCa applications and services.

<div class="row row-cols-1 row-cols-md-2 g-4 my-1">
  <div class="col">
    <div class="card h-100">
      <div class="card-body d-flex flex-column">
        <h3 class="card-title">Connection API</h3>
        <p class="card-text">Design and code-check steel connections from C#, Python, or any REST client. A locally hosted OpenAPI service drives the IDEA StatiCa Connection CBFEM solver: open projects, apply templates, set parameters and load effects, calculate, and generate reports.</p>
        <div class="mt-auto">
          <a href="docs/api/connection-api/connection_api_getting_started.md" class="btn btn-primary btn-sm">Get started</a>
          <a href="docs/api/connection-api/connection_api_overview.md" class="btn btn-outline-secondary btn-sm">Overview</a>
        </div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card h-100">
      <div class="card-body d-flex flex-column">
        <h3 class="card-title">RCS API</h3>
        <p class="card-text">Check reinforced concrete cross-sections from your own applications and scripts. Available with the same client model as the Connection API: official SDKs for C#/.NET and Python over a local REST service.</p>
        <div class="mt-auto">
          <a href="docs/api/rcs-api/rcs_api_getting_started.md" class="btn btn-primary btn-sm">Get started</a>
        </div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card h-100">
      <div class="card-body d-flex flex-column">
        <h3 class="card-title">BIM API — Checkbot links</h3>
        <p class="card-text">Integrate your FEA or CAD application with IDEA StatiCa Checkbot. The BIM API Link framework gives third-party developers model export, library conversions, and model syncing out of the box.</p>
        <div class="mt-auto">
          <a href="docs/bimapi/bimapi_checkbot_link.md" class="btn btn-primary btn-sm">Get started</a>
        </div>
      </div>
    </div>
  </div>
  <div class="col">
    <div class="card h-100">
      <div class="card-body d-flex flex-column">
        <h3 class="card-title">IDEA Open Model (IOM)</h3>
        <p class="card-text">The open data format at the heart of IDEA StatiCa interoperability. Describe structural models — geometry, cross-sections, loading, connection data — in XML and exchange them with IDEA StatiCa applications.</p>
        <div class="mt-auto">
          <a href="docs/iom/iom_getting_started.md" class="btn btn-primary btn-sm">Get started</a>
        </div>
      </div>
    </div>
  </div>
</div>

## Extensions

Low-code tools built on top of the APIs and IOM — for structural engineers and computational designers automating workflows through visual programming:

* [Plugin for Rhino/Grasshopper](docs/extensions/grasshopper/grasshopper_overview.md)

## Get help

* [Ask on the forum (GitHub Discussions)](https://github.com/idea-statica/ideastatica-public/discussions) — questions, support, and feature requests.
* [Submit an issue](https://github.com/idea-statica/ideastatica-public/issues) — bug reports for the SDKs, examples, and this documentation.

## For AI tools

* Machine-readable index of this site: [llms.txt](https://developer.ideastatica.com/llms.txt)
* [Connection API LLM reference](docs/api/connection-api/connection_api_llm_reference.md) — a single-file SDK reference designed for AI assistant context windows.
* Using ChatGPT? Try the official [IDEA StatiCa Connection API Assistant for Python](https://chatgpt.com/g/g-69ba8672efa081918789abcd0acb8d6b-idea-statica-connection-api-assistant-for-python).
