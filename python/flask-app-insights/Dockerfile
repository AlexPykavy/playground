FROM python:3.9-slim AS build

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends build-essential

# setup & activate venv
RUN python -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

COPY requirements.txt .
RUN pip install --no-cache-dir --no-deps -r requirements.txt

FROM python:3.9-slim

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl procps

# activate venv
ENV PATH="/opt/venv/bin:$PATH"

COPY --from=build /opt/venv /opt/venv
COPY . .

ARG USERNAME=python
RUN groupadd --gid 10001 $USERNAME \
    && useradd --uid 10000 --gid $USERNAME --shell /bin/bash --create-home $USERNAME
USER $USERNAME

CMD ["uwsgi", "--ini", "/app/wsgi.ini"]
