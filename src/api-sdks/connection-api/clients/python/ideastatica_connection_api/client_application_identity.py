from typing import Optional


class ClientApplicationIdentity:
    """The name an application gives itself towards the Connection REST API service.

    Why it exists: the service reports every endpoint call it serves, and without this every call
    looks the same. It cannot tell one python script from another, from a Grasshopper component,
    from a partner's tool, because the only thing a client is identified by is the ClientId it was
    handed, which is a fresh GUID per session.

    One string in a request header answers it for every client. Sending nothing keeps the previous
    behaviour: the calls are counted, just not attributed.

    It identifies the APPLICATION, not the user: it is meant to be a constant of the release, and
    nothing about the machine, the project or the person belongs in it.
    """

    HEADER_NAME = "X-Idea-Client-App"
    """The request header the service reads the identification from. The C# client holds the same
    string in ``ClientApplicationIdentity.HeaderName``: it is a wire contract between separately
    shipped sides.

    A caller that sets this header itself, without :meth:`format`, has to keep the value printable
    ASCII: a byte above 0x7F in a header value makes the service's HTTP stack answer with a bare
    400 before the service is reached, so a name with a diacritic fails every call rather than
    arriving mangled.
    """

    MAX_LENGTH = 64
    """The longest value that is sent. It is a name, not a payload, and it travels on every
    request; anything longer is cut."""

    @staticmethod
    def format(application: Optional[str], version: Optional[str] = None) -> Optional[str]:
        """The header value for an application name and an optional version, or ``None`` when
        there is nothing to send.

        The result is restricted to printable ASCII. A byte above 0x7F costs the whole request -
        the service's HTTP stack answers a bare 400 before the service itself is reached - and a
        control character below 0x20 has no business in a value that ends up in a log line.
        Anything outside the range becomes ``_``, so a name never fails a call and stays
        recognisable rather than disappearing.

        :param application: For example ``"NorsokChecker"``. Optional.
        :param version: For example ``"1.4.2"``. Optional.
        """
        name = ClientApplicationIdentity._sanitize(application)
        if not name:
            return None

        suffix = ClientApplicationIdentity._sanitize(version)
        value = name if not suffix else f"{name}/{suffix}"

        return value if len(value) <= ClientApplicationIdentity.MAX_LENGTH \
            else value[:ClientApplicationIdentity.MAX_LENGTH]

    @staticmethod
    def _sanitize(text: Optional[str]) -> str:
        if text is None or not text.strip():
            return ""

        # 0x20-0x7E is printable ASCII; the separator is reserved for joining name and version
        return "".join(c if " " <= c <= "~" and c != "/" else "_" for c in text.strip())
