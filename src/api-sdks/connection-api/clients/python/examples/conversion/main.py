import os
import logging

import ideastatica_connection_api.connection_api_service_attacher as connection_api_service_attacher
from ideastatica_connection_api.models.country_code import CountryCode

# ── Setup ─────────────────────────────────────────────────────────────
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

baseUrl = "http://localhost:5000"
dir_path = os.path.dirname(os.path.realpath(__file__))
models_path = os.path.join(dir_path, "models")

# ── Connect to API ────────────────────────────────────────────────────
with connection_api_service_attacher.ConnectionApiServiceAttacher(baseUrl).create_api_client() as api_client:
    try:
        # ── Find all .ideaCon files ────────────────────────────────────
        files = [
            f for f in os.listdir(models_path)
            if f.endswith(".ideaCon") and not f.endswith("_AISC.ideaCon")
        ]

        files = sorted(files)

        print(f"\nFound {len(files)} file(s) to process.\n")

        if not files:
            print("No .ideaCon files found.")
            exit()

        # ── Process each file ─────────────────────────────────────────
        for file in files:
            project_id = None

            try:
                project_file_path = os.path.join(models_path, file)
                output_file = file.replace(".ideaCon", "_AISC.ideaCon")
                output_file_path = os.path.join(models_path, output_file)

                print(f"────────────────────────────────────────────")
                print(f"Processing: {file}")

                # ── Open project ──
                api_client.project.open_project_from_filepath(project_file_path)
                project_id = api_client.project.active_project_id

                print(f"[DIAG] project_id = {project_id}")

                if not project_id:
                    raise RuntimeError("Project opened but project_id is None.")

                # ── Conversion ──
                default_mapping = api_client.conversion.get_conversion_mapping(
                    project_id,
                    CountryCode.AMERICAN
                )

                # Keep same cross-sections
                for css in default_mapping.cross_sections:
                    css.target_value = css.source_value

                api_client.conversion.change_code(project_id, default_mapping)

                # ── Save project ──
                api_client.project.download_project(project_id, output_file_path)

                print(f"Saved: {output_file}")

            except Exception as e:
                print(f"❌ Error processing {file}: {e}")

            finally:
                # ── Always close project ──
                if project_id:
                    try:
                        api_client.project.close_project(project_id)
                        logger.info(f"Closed project {project_id}")
                    except Exception as close_err:
                        logger.warning(f"Could not close project: {close_err}")

        print("\n✅ All files processed.")

    except Exception as e:
        print(f"\nOperation failed: {e}")
        raise
    finally:
        # Prevent __exit__ from trying to close an already-closed project
        api_client.project = None