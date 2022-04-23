#!/bin/bash
mysql -v -v -u user -p cloudx_course < /var/develop/scripts/sql/create_tables.sql > /var/develop/out/create_tables.txt