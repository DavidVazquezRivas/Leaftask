import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from '../config.js';
import { headers } from '../setup.js';

export const organizationsScenario = {
    read(data) {
        const { token, orgId } = data;

        const list = http.get(`${BASE_URL}/organizations`, { headers: headers(token) });
        check(list, { 'list orgs 200': (r) => r.status === 200 });

        const detail = http.get(`${BASE_URL}/organizations/${orgId}`, { headers: headers(token) });
        check(detail, { 'get org 200': (r) => r.status === 200 });

        const perms = http.get(`${BASE_URL}/organizations/${orgId}/permissions/me`, { headers: headers(token) });
        check(perms, { 'get org perms 200': (r) => r.status === 200 });

        sleep(0.5);
    },
};
