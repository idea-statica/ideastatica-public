import sys
import os

# Get the parent directory
parent_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))

# Add the parent directory to sys.path, ahead of anything already installed - these assert what
# the repository holds, and a pip-installed copy of the package would otherwise answer instead.
sys.path.insert(0, parent_dir)

from ideastatica_connection_api.client_application_identity import ClientApplicationIdentity

# What an application is allowed to call itself towards the service. The value travels in a request
# header on every call, so the rules are about what a header can safely carry. These need no running
# service.


def test_an_application_and_its_version_are_joined_by_a_slash():
    assert ClientApplicationIdentity.format("NorsokChecker", "1.4.2") == "NorsokChecker/1.4.2"
    # the version is optional
    assert ClientApplicationIdentity.format("NorsokChecker") == "NorsokChecker"
    # trimmed, because a name is typed by a developer
    assert ClientApplicationIdentity.format(" NorsokChecker ", " 1.4.2 ") == "NorsokChecker/1.4.2"


def test_without_a_name_there_is_no_header_to_send():
    # Nothing to send is not an error - it is the previous behaviour, where calls are counted but
    # not attributed.
    assert ClientApplicationIdentity.format(None) is None
    assert ClientApplicationIdentity.format("") is None
    assert ClientApplicationIdentity.format("   ") is None
    # a version without a name identifies nothing
    assert ClientApplicationIdentity.format(None, "1.4.2") is None


def test_anything_a_header_cannot_carry_is_replaced_rather_than_sent():
    # A careless name must not be able to break every request the client makes: a byte above 0x7F
    # is answered with a bare 400 by the service's HTTP stack, and a control character is mangled by
    # whatever handles it. The value stays recognisable rather than disappearing.
    assert ClientApplicationIdentity.format("Norsok\r\nChecker") == "Norsok__Checker"
    assert ClientApplicationIdentity.format("Norsok\tChecker") == "Norsok_Checker"
    # printable ASCII only
    assert ClientApplicationIdentity.format("Kontrola spojů") == "Kontrola spoj_"
    # the slash is reserved for joining the version
    assert ClientApplicationIdentity.format("Norsok/Checker", "1.0") == "Norsok_Checker/1.0"


def test_a_long_name_is_cut_rather_than_sent_whole():
    value = ClientApplicationIdentity.format("x" * 100, "1.0")

    assert len(value) == ClientApplicationIdentity.MAX_LENGTH
    assert value == "x" * ClientApplicationIdentity.MAX_LENGTH


def test_an_unnamed_client_sends_no_header():
    # The header is what the service reads; a client that was given no name must not send the
    # header at all, rather than send it empty.
    from ideastatica_connection_api.connection_api_client import ConnectionApiClient

    assert ConnectionApiClient("http://localhost:5000").client_application is None
    assert ConnectionApiClient("http://localhost:5000", "NorsokChecker",
                               "1.4.2").client_application == "NorsokChecker/1.4.2"
