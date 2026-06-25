import { THRESHOLDS } from './config.js';
import { setup as setupFn, teardown as teardownFn } from './setup.js';
import { workitemsScenario } from './scenarios/workitems.js';
import { organizationsScenario } from './scenarios/organizations.js';
import { projectsScenario } from './scenarios/projects.js';
import { commentsScenario } from './scenarios/comments.js';

export const options = {
    thresholds: THRESHOLDS,
    scenarios: {
        workitems_read: {
            executor: 'constant-vus',
            vus: 15,
            duration: '2m',
            exec: 'workitemsRead',
        },
        workitems_write: {
            executor: 'constant-vus',
            vus: 5,
            duration: '2m',
            exec: 'workitemsWrite',
        },
        projects_read: {
            executor: 'constant-vus',
            vus: 5,
            duration: '2m',
            exec: 'projectsRead',
        },
        organizations_read: {
            executor: 'constant-vus',
            vus: 3,
            duration: '2m',
            exec: 'organizationsRead',
        },
        comments_rw: {
            executor: 'constant-vus',
            vus: 2,
            duration: '2m',
            exec: 'commentsReadWrite',
        },
    },
};

export const setup = setupFn;
export const teardown = teardownFn;

export function workitemsRead(data) { workitemsScenario.read(data); }
export function workitemsWrite(data) { workitemsScenario.write(data); }
export function projectsRead(data) { projectsScenario.read(data); }
export function organizationsRead(data) { organizationsScenario.read(data); }
export function commentsReadWrite(data) { commentsScenario.readWrite(data); }
