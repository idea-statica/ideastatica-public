from setuptools import setup, find_packages

NAME = "ideastatica_setup_tools"
VERSION = "0.1"
PYTHON_REQUIRES = ">= 3.8"
REQUIRES = []

setup(
    name=NAME,
    version=VERSION,
    author="IdeaStatiCa",
    keywords=["IdeaStatiCa"],
    packages=find_packages(),
    install_requires=REQUIRES,
)