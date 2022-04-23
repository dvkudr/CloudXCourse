#!/bin/bash
mysql -v -v -u user -p cloudx_course < /var/develop/scripts/sql/drop_tables.sql > /var/develop/out/drop_tables.txt