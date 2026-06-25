import http from 'k6/http';
import { check, sleep } from 'k6';
import { BASE_URL } from '../config.js';
import { headers } from '../setup.js';

export const projectsScenario = {
    read(data) {
        const { token, orgId, projectId } = data;

        const list = http.get(`${BASE_URL}/projects`, { headers: headers(token) });
        check(list, { 'list projects 200': (r) => r.status === 200 });

        const byOrg = http.get(`${BASE_URL}/projects/organization/${orgId}`, { headers: headers(token) });
        check(byOrg, { 'list org projects 200': (r) => r.status === 200 });

        const detail = http.get(`${BASE_URL}/projects/${projectId}`, { headers: headers(token) });
        check(detail, { 'get project 200': (r) => r.status === 200 });

        const members = http.get(`${BASE_URL}/projects/${projectId}/members`, { headers: headers(token) });
        check(members, { 'get project members 200': (r) => r.status === 200 });

        sleep(0.5);
    },
};
