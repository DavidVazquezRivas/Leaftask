import { THRESHOLDS } from './config.js';
import { setup as setupFn, teardown as teardownFn } from './setup.js';
import { workitemsScenario } from './scenarios/workitems.js';
import { organizationsScenario } from './scenarios/organizations.js';
import { projectsScenario } from './scenarios/projects.js';
import { commentsScenario } from './scenarios/comments.js';

const isSmoke = __ENV.PROFILE === 'smoke';
const isStress = __ENV.PROFILE === 'stress';

function scenario(exec, vus, duration) {
    const stressVus = Math.round(vus * 1.6);
    return {
        executor: 'constant-vus',
        vus: isSmoke ? 1 : (isStress ? stressVus : vus),
        duration: isSmoke ? '30s' : (isStress ? '4m' : duration),
        exec,
    };
}

export const options = {
    thresholds: THRESHOLDS,
    scenarios: {
        workitems_read:     scenario('workitemsRead',     15, '2m'),
        workitems_write:    scenario('workitemsWrite',     5, '2m'),
        projects_read:      scenario('projectsRead',       5, '2m'),
        organizations_read: scenario('organizationsRead',  3, '2m'),
        comments_rw:        scenario('commentsReadWrite',  2, '2m'),
    },
};

export const setup = setupFn;
export const teardown = teardownFn;

export function workitemsRead(data) { workitemsScenario.read(data); }
export function workitemsWrite(data) { workitemsScenario.write(data); }
export function projectsRead(data) { projectsScenario.read(data); }
export function organizationsRead(data) { organizationsScenario.read(data); }
export function commentsReadWrite(data) { commentsScenario.readWrite(data); }
